using System.IO.Compression;
using System.Reflection;
using BaseUtils;
using Feed.SEC._sys;
using Feed.SEC._sys.Logic;
using Feed.SEC._sys.Rows;
using Feed.SEC._sys.Rows.Base;
using Feed.SEC._sys.Rows.Logic;
using Feed.SEC._sys.Rows.Utils;
using Feed.SEC._sys.RowsStringy;
using Feed.SEC._sys.Utils;
using LINQPad;

namespace Feed.SEC;

[Flags]
enum Step
{
	Download = 1,
	Clean = 2,
	Group = 4,
	Rename = 8,

	All = Download | Clean | Group | Rename,
}

static class APIDev
{
	public static void LogRootFolder()
	{
		var str = $"Root folder: {Consts.RootFolder}";
		Console.WriteLine(str);
		Console.WriteLine(new string('=', str.Length));
		Console.WriteLine();
	}

	public static void Run(Step step = Step.All)
	{
		if (step.HasFlag(Step.Download)) _1_Download.Run();
		if (step.HasFlag(Step.Clean)) _2_Clean.Run();
		if (step.HasFlag(Step.Group)) _3_Group.Run();
		if (step.HasFlag(Step.Rename)) _4_NameChangeCompiler.Run();
		Console.WriteLine();
		Console.WriteLine("FINISHED");
	}

	public static void Check(Step step = Step.All)
	{
		if (step.HasFlag(Step.Download)) Check_Download();
		if (step.HasFlag(Step.Clean)) Check_Clean();
		if (step.HasFlag(Step.Group)) Check_Group();
	}

	public static string[] GetCompanies() => Consts.Group.GetAllCompanies();

	public static NameChangeInfos GetNameChanges() => JsonUtils.Load<NameChangeInfos>(Consts.Rename.DataFile);

	public static IEnumerable<T> Load_Download_Rows<T>() where T : IStringyRow<T> => StreamStringyRowsInArchives<T>(Consts.Download.GetAllZipFiles(), LineMethod.OriginalEscaping);

	public static IEnumerable<T> Load_Clean_StringyRows<T>() where T : IStringyRow<T> => StreamStringyRowsInArchives<T>(Consts.Clean.GetAllZipFiles(), LineMethod.ReplaceTabChar);

	public static IEnumerable<T> Load_Clean_Rows<T>() where T : IRow<T> => StreamRowsInArchives<T>(Consts.Clean.GetAllZipFiles());

	public static IEnumerable<T> Load_Group_Rows<T>(string company) where T : IRow<T> => StreamRowsInArchives<T>([Consts.Group.CompanyZipFile(company)]);
	public static IEnumerable<T> Load_All_Group_Rows<T>() where T : IRow<T> => StreamRowsInArchives<T>(Consts.Group.GetAllCompanyZipFiles());

	public static RowSet Load_Group_RowSet(string company) => RowReader.ReadRowSet(Consts.Group.CompanyZipFile(company)).EnsureConsistent();

	public static StmtHistory Load_ReferenceData(string company) => JsonUtils.Load<StmtHistory>(Consts.ReferenceData.CompanyJsonFile(company));
	public static void Save_ReferenceData(string company, StmtHistory history)
	{
		var jsonFile = Consts.ReferenceData.CompanyJsonFile(company);
		var all = JsonUtils.LoadOr(jsonFile, StmtHistory.Empty);
		foreach (var (q, s) in history.IncomeStatements)
			all.IncomeStatements[q] = s;
		foreach (var (q, s) in history.BalanceSheets)
			all.BalanceSheets[q] = s;
		foreach (var (q, s) in history.CashFlows)
			all.CashFlows[q] = s;
		all.Save(jsonFile);
	}


	static void Check_Download()
	{
		var archFiles = Consts.Download.GetAllZipFiles();
		foreach (var archFile in archFiles)
		{
			RowArchiveChecker.CheckAsStringyRows(archFile, LineMethod.OriginalEscaping);
		}
		Console.WriteLine("Download -> OK");
	}


	static void Check_Clean()
	{
		var archFiles = Consts.Clean.GetAllZipFiles();
		foreach (var archFile in archFiles)
		{
			//RowArchiveChecker.CheckAsStringyRows(archFile, LineMethod.ReplaceTabChar);
			RowArchiveChecker.CheckAsNormalRows(archFile);
		}
		Console.WriteLine("Clean    -> OK");
	}

	static void Check_Group()
	{
		var archFiles = Consts.Group.GetAllCompanyZipFiles();
		for (var index = 0; index < archFiles.Length; index++)
		{
			var archFile = archFiles[index];
			//RowArchiveChecker.CheckAsStringyRows(archFile, LineMethod.ReplaceTabChar);
			RowArchiveChecker.CheckAsNormalRows(archFile);
			if (index % 100 == 0)
				Console.WriteLine($"    Progress {index + 1}/{archFiles.Length} ");
		}

		Console.WriteLine("Group    -> OK");
	}





	static IEnumerable<T> StreamStringyRowsInArchives<T>(string[] archFiles, LineMethod method) where T : IStringyRow<T>
	{
		Util.Progress = 0;
		for (var i = 0; i < archFiles.Length; i++)
		{
			var archFile = archFiles[i];
			using var arch = ZipFile.OpenRead(archFile);
			var entry = arch.GetEntry(T.NameInArchive) ?? throw new ArgumentException("Invalid NameInArchive");
			using var stream = entry.Open();
			using var bs = new BufferedStream(stream);
			using var sr = new StreamReader(bs);
			foreach (var row in StreamStringyRows<T>(sr, T.ColumnCount, archFile, T.NameInArchive, method))
				yield return row;
			Util.Progress = (int)((i + 1) * 100.0 / archFiles.Length);
		}
		Util.Progress = null;
	}


	static IEnumerable<T> StreamStringyRows<T>(StreamReader sr, int columnCount, string archFile, string nameInArchive, LineMethod method) where T : IStringyRow<T>
	{
		var isFirst = true;
		var lineIdx = 0;
		while (sr.ReadLine() is { } line)
		{
			lineIdx++;
			if (isFirst)
			{
				isFirst = false;
				continue;
			}

			var xs = LineIO.Line2Fields(line, columnCount, method);
			var loc = new Loc(archFile, nameInArchive, lineIdx);
			if (xs.Length != columnCount)
				throw new InvalidOperationException($"Invalid column count @ {loc}");
			var row = T.Parse(loc, xs);
			yield return row;
		}
	}



	static IEnumerable<T> StreamRowsInArchives<T>(string[] archFiles) where T : IRow<T>
	{
		Util.Progress = 0;
		for (var i = 0; i < archFiles.Length; i++)
		{
			var archFile = archFiles[i];
			using var arch = ZipFile.OpenRead(archFile);
			var entry = arch.GetEntry(T.NameInArchive) ?? throw new ArgumentException("Invalid NameInArchive");
			using var stream = entry.Open();
			using var bs = new BufferedStream(stream);
			using var sr = new StreamReader(bs);
			foreach (var row in StreamRows<T>(sr, T.ColumnCount))
				yield return row;
			Util.Progress = (int)((i + 1) * 100.0 / archFiles.Length);
		}
		Util.Progress = null;
	}


	static IEnumerable<T> StreamRows<T>(StreamReader sr, int columnCount) where T : IRow<T>
	{
		var isFirst = true;
		while (sr.ReadLine() is { } line)
		{
			if (isFirst)
			{
				isFirst = false;
				continue;
			}

			var xs = LineIO.Line2Fields(line, columnCount, LineMethod.ReplaceTabChar);
			if (xs.Length != columnCount)
				throw new InvalidOperationException($"Invalid column count {xs.Length} != {columnCount}");
			var row = T.Parse(xs);
			yield return row;
		}
	}
}