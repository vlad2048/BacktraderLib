using Feed.SEC._sys.Utils;
using System.IO.Compression;
using BaseUtils;
using Feed.SEC._sys.Logic;
using Feed.SEC._sys.Rows;
using Feed.SEC._sys.RowsStringy;

namespace Feed.SEC._sys;

static class _2_Clean
{
	public static void Run()
	{
		var Log = Logger.Make(LogCategory._2_Clean);

		var quartersNext = Consts.Download.GetAllQuarters();
		var quartersPrev = Consts.Clean.GetAllQuarters();
		var quartersNew = quartersNext.ExceptA(quartersPrev);

		quartersNew.Loop(Log, 2, "Clean", x => Consts.Download.QuarterZipFile(x).FmtArchFile(), quarterNew =>
		{
			var archFileSrc = Consts.Download.QuarterZipFile(quarterNew);
			var archFileDst = Consts.Clean.QuarterZipFile(quarterNew);

			var rowsToRemove = FindRowsToRemove(archFileSrc, Log);

			RemoveRows(archFileSrc, archFileDst, rowsToRemove);

			try
			{
				RowArchiveChecker.CheckAsStringyRows(archFileDst, LineMethod.ReplaceTabChar);
			}
			catch (Exception)
			{
				File.Delete(archFileDst);
				throw;
			}
		});
	}






	// **************************
	// **************************
	// **** FindRowsToRemove ****
	// **************************
	// **************************
	static Loc[] FindRowsToRemove(string archFileSrc, Action<string> Log)
	{
		var rowsToRemove = new List<Loc>();

		var (nums, pres, subs, tags) = StringyRowReader.ReadRowSet(archFileSrc, LineMethod.OriginalEscaping);
		var stats = new Stats(Log, subs, nums, tags, pres);


		(subs, stats.Sub_Invalid) = InvalidRow_Remove(subs, rowsToRemove, SubRow.IsParsable);
		(nums, stats.Num_Invalid) = InvalidRow_Remove(nums, rowsToRemove, NumRow.IsParsable);
		(tags, stats.Tag_Invalid) = InvalidRow_Remove(tags, rowsToRemove, TagRow.IsParsable);
		(pres, stats.Pre_Invalid) = InvalidRow_Remove(pres, rowsToRemove, PreRow.IsParsable);


		(nums, stats.Num_Duplicate) = DuplicateKeys_Remove(nums, e => e.Key, rowsToRemove);
		DuplicateKeys_EnsureNone(subs, e => e.Key);
		DuplicateKeys_EnsureNone(tags, e => e.Key);
		DuplicateKeys_EnsureNone(pres, e => e.Key);

		var subKeys = subs.Select(e => e.Key).ToHashSet();
		var tagKeys = tags.Select(e => e.Key).ToHashSet();

		(nums, stats.Num_NoSub) = ForeignKeyViolation_RemoveSrcRows<NumStringyRow, SubStringyRow, SubStringyRowKey>(nums, e => e.Key.SubKey, subKeys, rowsToRemove);
		(nums, stats.Num_NoTag) = ForeignKeyViolation_RemoveSrcRows<NumStringyRow, TagStringyRow, TagStringyRowKey>(nums, e => e.Key.TagKey, tagKeys, rowsToRemove);
		(pres, stats.Pre_NoSub) = ForeignKeyViolation_RemoveSrcRows<PreStringyRow, SubStringyRow, SubStringyRowKey>(pres, e => e.Key.SubKey, subKeys, rowsToRemove);
		(pres, stats.Pre_NoTag) = ForeignKeyViolation_RemoveSrcRows<PreStringyRow, TagStringyRow, TagStringyRowKey>(pres, e => e.TagKey, tagKeys, rowsToRemove);

		stats.Pre_NoSubTag = PartialKeyViolation_RemoveSrcRows<PreStringyRow, NumStringyRow, (SubStringyRowKey, TagStringyRowKey)>(pres, e => (e.Key.SubKey, e.TagKey), nums, e => (e.Key.SubKey, e.Key.TagKey), rowsToRemove);


		stats.Log();

		return [..rowsToRemove];
	}



	static (T[], int) DuplicateKeys_Remove<T, K>(T[] rows, Func<T, K> keyFun, List<Loc> rowsToRemove) where T : IStringyRow<T>
	{
		var grps = rows.GroupBy(keyFun).ToArray();
		var (dups, uniqs) = (grps.Where(e => e.Count() > 1).SelectManyA(), grps.Where(e => e.Count() == 1).SelectManyA());

		rowsToRemove.AddRange(dups.Select(e => e.Loc));

		return (uniqs, dups.Length);
	}


	static void DuplicateKeys_EnsureNone<T, K>(T[] rows, Func<T, K> keyFun) where T : IStringyRow<T>
	{
		var grps = rows.GroupBy(keyFun).ToArray();
		var dups = grps.Where(e => e.Count() > 1).SelectManyA();

		if (dups.Length > 0)
		{
			StringyRowReader.PrintLines(dups.SelectA(e => e.Loc), T.ColumnCount, Console.WriteLine);
			throw new ArgumentException($"Duplicate keys in {typeof(T).Name}");
		}
	}




	static (TSrc[], int) ForeignKeyViolation_RemoveSrcRows<TSrc, TDst, TDstK>(TSrc[] srcRows, Func<TSrc, TDstK> refFun, HashSet<TDstK> dstKeys, List<Loc> rowsToRemove) where TSrc : IStringyRow<TSrc> where TDst : IStringyRow<TDst>
	{
		var broken = srcRows.WhereA(e => !dstKeys.Contains(refFun(e)));
		var valid = srcRows.WhereA(e => dstKeys.Contains(refFun(e)));
		//if (broken.Length > 0) StringyRowReader.PrintLines(broken.SelectA(e => e.Loc), TSrc.ColumnCount, ctx.LogVerbose);
		rowsToRemove.AddRange(broken.Select(e => e.Loc));
		return (valid, broken.Length);
	}

	/*static void ForeignKeyViolation_EnsureNone<TSrc, TDst, TDstK>(TSrc[] srcRows, Func<TSrc, TDstK> refFun, HashSet<TDstK> dstKeys) where TSrc : IStringyRow<TSrc> where TDst : IStringyRow<TDst>
	{
		var broken = srcRows.WhereA(e => !dstKeys.Contains(refFun(e)));

		if (broken.Length > 0)
		{
			StringyRowReader.PrintLines(broken.SelectA(e => e.Loc), TSrc.ColumnCount, Console.WriteLine);
			throw new ArgumentException($"Foreign key violation in {typeof(TSrc).Name} -> {typeof(TDst).Name}");
		}
	}*/




	static int PartialKeyViolation_RemoveSrcRows<TSrc, TDst, PartialK>(TSrc[] srcRows, Func<TSrc, PartialK> srcFun, TDst[] dstRows, Func<TDst, PartialK> dstFun, List<Loc> rowsToRemove) where TSrc : IStringyRow<TSrc> where TDst : IStringyRow<TDst>
	{
		var dstKeys = dstRows.Select(dstFun).ToHashSet();
		var broken = srcRows.WhereA(e => !dstKeys.Contains(srcFun(e)));
		//if (broken.Length > 0) StringyRowReader.PrintLines(broken.SelectA(e => e.Loc), TSrc.ColumnCount, ctx.LogVerbose);
		rowsToRemove.AddRange(broken.Select(e => e.Loc));
		return broken.Length;
	}



	static (T[], int) InvalidRow_Remove<T>(T[] rows, List<Loc> rowsToRemove, Func<T, bool> isValid) where T : IStringyRow<T>
	{
		var invalid = rows.WhereA(e => !isValid(e));
		var valid = rows.WhereA(isValid);
		//if (invalid.Length > 0) StringyRowReader.PrintLines(invalid.SelectA(e => e.Loc), T.ColumnCount, ctx.LogVerbose);
		rowsToRemove.AddRange(invalid.Select(e => e.Loc));
		return (valid, invalid.Length);
	}





	// ********************
	// ********************
	// **** RemoveRows ****
	// ********************
	// ********************
	static void RemoveRows(string archFileSrc, string archFileDst, Loc[] rowsToRemove)
	{
		var locsMap = rowsToRemove.GroupBy(e => e.NameInArchive).ToDictionary(e => e.Key, e => e.ToArray());

		using var archSrc = ZipFile.Open(archFileSrc, ZipArchiveMode.Read);
		using var archDst = ZipFile.Open(archFileDst, ZipArchiveMode.Create);

		foreach (var entrySrc in archSrc.Entries)
		{
			if (entrySrc.Name == "readme.htm")
				continue;

			var lineIdxToRemoveSet = locsMap.GetValueOrDefault(entrySrc.Name, []).Select(e => e.Line).ToHashSet();

			var entrySrcStream = entrySrc.Open();
			using var bsSrc = new BufferedStream(entrySrcStream);
			using var streamSrc = new StreamReader(bsSrc);

			var entryDst = archDst.CreateEntry(entrySrc.Name, CompressionLevel.Optimal);
			var entryDstStream = entryDst.Open();
			using var bsDst = new BufferedStream(entryDstStream);
			using var streamDst = new StreamWriter(bsDst)
			{
				NewLine = "\n",
			};

			var lineIdx = 0;
			var isFirst = true;
			while (streamSrc.ReadLine() is { } line)
			{
				lineIdx++;
				if (!lineIdxToRemoveSet.Contains(lineIdx))
				{
					if (isFirst)
					{
						isFirst = false;
						streamDst.WriteLine(line);
					}
					else
					{
						var fieldsSrc = LineIO.Line2Fields(line, null, LineMethod.OriginalEscaping);
						var lineDst = LineIO.Fields2Line(fieldsSrc, LineMethod.ReplaceTabChar);
						streamDst.WriteLine(lineDst);
					}
				}
			}
		}
	}











	// ***************
	// ***************
	// **** Stats ****
	// ***************
	// ***************
	sealed class Stats
	{
	   	readonly Action<string> logger;
	   
	   	readonly int subTotal;
	   	readonly int numTotal;
	   	readonly int tagTotal;
	   	readonly int preTotal;
	   
	   	public int Sub_Invalid { get; set; }
	   	public int Num_Invalid { get; set; }
	   	public int Tag_Invalid { get; set; }
	   	public int Pre_Invalid { get; set; }
	   
	   	public int Num_Duplicate { get; set; }
	   
	   	public int Num_NoSub { get; set; }
	   	public int Num_NoTag { get; set; }
	   	public int Pre_NoSub { get; set; }
	   	public int Pre_NoTag { get; set; }
	   
	   	public int Pre_NoSubTag { get; set; }
	   
	   	public Stats(Action<string> logger, SubStringyRow[] subs, NumStringyRow[] nums, TagStringyRow[] tags, PreStringyRow[] pres) =>
	   		(this.logger, subTotal, numTotal, tagTotal, preTotal) = (logger, subs.Length, nums.Length, tags.Length, pres.Length);
	   
	   	public void Log()
	   	{
	   		// @formatter:off
	   		LDel("[Sub].Invalid",	Sub_Invalid);
	   		LDel("[Num].Invalid",	Num_Invalid);
	   		LDel("[Num].Duplicate",	Num_Duplicate);
	   		LDel("[Num].NoSub",		Num_NoSub);
	   		LDel("[Num].NoTag",		Num_NoTag);
	   		LDel("[Tag].Invalid",	Tag_Invalid);
	   		LDel("[Pre].Invalid",	Pre_Invalid);
	   		LDel("[Pre].NoSub",		Pre_NoSub);
	   		LDel("[Pre].NoTag",		Pre_NoTag);
	   		LDel("[Pre].NoSubTag",	Pre_NoSubTag);
			// @formatter:on

			var subDel = Sub_Invalid;
			var numDel = Num_Invalid + Num_Duplicate + Num_NoSub + Num_NoTag;
			var tagDel = Tag_Invalid;
			var preDel = Pre_Invalid + Pre_NoSub + Pre_NoTag + Pre_NoSubTag;
			var tot = subTotal + numTotal + tagTotal + preTotal;
			var del = subDel + numDel + tagDel + preDel;

			if (del > 0)
				L("");
			L($"              Downloaded        Removed");
			L($"----------------------------------------------------");
			L($"[Sub]       {subTotal.fn()}   {subDel.fn()}     {subDel.fp(subTotal)}");
			L($"[Num]       {numTotal.fn()}   {numDel.fn()}     {numDel.fp(numTotal)}");
			L($"[Tag]       {tagTotal.fn()}   {tagDel.fn()}     {tagDel.fp(tagTotal)}");
			L($"[Pre]       {preTotal.fn()}   {preDel.fn()}     {preDel.fp(preTotal)}");
			L($"----------------------------------------------------");
			L($"            {     tot.fn()}   {   del.fn()}     {del.fp(tot)}");
	   	}

		void L(string s) => logger($"    {s}");
	   	void LDel(string name, int num)
	   	{
	   		if (num == 0) return;
	   		L($"{name,-20}{num,9:n0}");
	   	}
	}
}


file static class CleanFmtUtils
{
	public static string fn(this int v) => v is 0 ? "_".PadLeft(12) : $"{v,12:n0}";
	public static string fp(this int del, int tot) => del is 0 ? "_   ".PadLeft(6) : $"{perc(del, tot),6:F2}%";
	static double perc(int del, int tot) => tot is 0 ? 0 : del * 100.0 / tot;
}
