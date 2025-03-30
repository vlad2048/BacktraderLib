using System.IO.Compression;
using Feed.SEC._sys.Logic;
using Feed.SEC._sys.Rows.Base;

namespace Feed.SEC._sys.Rows.Utils;


static class RowReader
{
	public static RowSet ReadRowSet(string archFile)
	{
		using var arch = ZipFile.OpenRead(archFile);
		var nums = ReadFromArchive<NumRow>(arch);
		var pres = ReadFromArchive<PreRow>(arch);
		var subs = ReadFromArchive<SubRow>(arch);
		var tags = ReadFromArchive<TagRow>(arch);
		return new RowSet(
			nums,
			pres,
			subs,
			tags
		);
	}

	public static void MergeRowSet(string archFile, RowSet rowSetNext)
	{
		if (rowSetNext.IsEmpty)
			return;

		var exists = File.Exists(archFile);
		var rowSetPrev = exists ? ReadRowSet(archFile) : RowSet.Empty;
		var rowSet = RowSetUtils.Merge([rowSetPrev, rowSetNext]);
		if (exists)
			File.Delete(archFile);

		using var arch = ZipFile.Open(archFile, ZipArchiveMode.Create);
		arch.SetRows(rowSet.Nums);
		arch.SetRows(rowSet.Pres);
		arch.SetRows(rowSet.Subs);
		arch.SetRows(rowSet.Tags);
	}

	
	static void SetRows<T>(this ZipArchive arch, T[] rows) where T : IRow<T>
	{
		var entry = arch.CreateEntry(T.NameInArchive);
		using var stream = entry.Open();
		using var bs = new BufferedStream(stream);
		using var sw = new StreamWriter(bs);
		sw.WriteLine(T.HeaderLine);
		foreach (var row in rows)
			sw.WriteLine(LineIO.Fields2Line(row.Fields, LineMethod.ReplaceTabChar));
	}


	static T[] ReadFromArchive<T>(ZipArchive arch) where T : IRow<T>
	{
		var entry = arch.GetEntry(T.NameInArchive) ?? throw new ArgumentException("Invalid NameInArchive");
		using var stream = entry.Open();
		var rows = Read<T>(stream);
		return rows.ToArray();
	}


	static IEnumerable<T> Read<T>(Stream stream) where T : IRow<T>
	{
		using var bs = new BufferedStream(stream);
		using var sr = new StreamReader(bs);
		var isFirst = true;
		while (sr.ReadLine() is { } line)
		{
			if (isFirst)
			{
				isFirst = false;
				continue;
			}
			yield return RowUtils.Parse<T>(line, LineMethod.ReplaceTabChar);
		}
	}
}