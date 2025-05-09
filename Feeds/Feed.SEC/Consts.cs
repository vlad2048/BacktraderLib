using BaseUtils;
using FeedUtils;

namespace Feed.SEC;


enum LogCategory
{
	_1_Download,
	_2_Clean,
	_3_Group,
	Pdf,
}


static class Consts
{
	static string RootFolder => API.RootFolder;
	public const int StepCount = 3;
	public static readonly HashSet<LogCategory> EnabledLogCategories =
	[
		LogCategory._1_Download,
		LogCategory._2_Clean,
		LogCategory._3_Group,
		LogCategory.Pdf,
	];


	public static class Download
	{
		static string Folder => Path.Combine(RootFolder, "1_download").CreateFolderIFN();
		public static string LastCheckFile => Path.Combine(Folder, "_last-check.json");
		public static string[] GetAllQuarters() => Directory.GetFiles(Folder, "*.zip").FromAllFilesSafe();
		public static string QuarterZipFile(string quarter) => Path.Combine(Folder, $"{quarter}.zip");
		public static string[] GetAllZipFiles() => GetAllQuarters().SelectA(QuarterZipFile);
		public static readonly TimeSpan CheckDelay = TimeSpan.FromDays(7);
	}


	public static class Clean
	{
		static string Folder => Path.Combine(RootFolder, "2_clean").CreateFolderIFN();
		public static string[] GetAllQuarters() => Directory.GetFiles(Folder, "*.zip").FromAllFilesSafe();
		public static string QuarterZipFile(string quarter) => Path.Combine(Folder, $"{quarter}.zip");
		public static string[] GetAllZipFiles() => GetAllQuarters().SelectA(QuarterZipFile);
	}


	public static class Group
	{
		static string Folder => Path.Combine(RootFolder, "3_group").CreateFolderIFN();
		public static string QuartersDoneFile => Path.Combine(Folder, "_quarters-done.json");
		public static string[] GetAllCompanies() => Directory.GetFiles(Folder, "*.zip").FromAllFilesSafe();
		public static string CompanyZipFile(string company) => Path.Combine(Folder, $"{company.ToFileSafe()}.zip");
		public static string[] GetAllCompanyZipFiles() => GetAllCompanies().SelectA(CompanyZipFile);

		// 1		45s
		// 2		29s
		// 3		23s
		// 4		21s
		// 8		21s
		public const int ParallelismLevel = 4;
	}


	public static class Rename
	{
		static string Folder => Path.Combine(RootFolder, "4_rename").CreateFolderIFN();

		public static string DataFile => Path.Combine(Folder, "data.json");
	}



	public static class Pdfs
	{
		static string Folder => Path.Combine(RootFolder, "pdfs").CreateFolderIFN();
		public static string File(string company, string adsh) => Path.Combine(Path.Combine(Folder, company.ToFileSafe()).CreateFolderIFN(), $"{adsh}.pdf");
	}



	public static class DumpCaps
	{
		public const int Tag = 32;
		//public const int Tag = int.MaxValue;
		public const int Tag_Tlabel = 16;
		public const int Tag_Doc = 32;

		public const int TagInNumPreXRef = 24;
	}
}
