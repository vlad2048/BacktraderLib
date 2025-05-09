using BaseUtils;
using Feed.SEC._sys;
using Feed.SEC._sys.Logic;
using Feed.SEC._sys.Rows;
using Feed.SEC._sys.RowsStringy;
using Feed.SEC._sys.Utils;
using Feed.SEC._sys.UtilsSteps;

namespace Feed.SEC;

public static class API
{
	static string? rootFolder { get; set; }
	internal static string RootFolder => rootFolder ?? throw new ArgumentException("Call Feed.SEC.API.Init() first");



	public static void Init(string rootFolder_) => rootFolder = rootFolder_;



	public static class Fetcher
	{
		public static void Run(Step step = Step.All)
		{
			if (step.HasFlag(Step.Download)) _1_Download.Run();
			if (step.HasFlag(Step.Clean)) _2_Clean.Run();
			if (step.HasFlag(Step.Group)) _3_Group.Run();
			Console.WriteLine();
			Console.WriteLine("FINISHED");
		}

		public static void Check(Step step = Step.All) => StepsChecker.Check(step);
	}



	public static class Utils
	{
		public static string GroupQuartersDoneFile => Consts.Group.QuartersDoneFile;
		public static string[] GetGroupCompanyFiles => Consts.Group.GetAllCompanyZipFiles();
		public static string[] GetCompanies() => Consts.Group.GetAllCompanies();
		public static NameChangeInfos GetNameChanges() => JsonUtils.Load<NameChangeInfos>(Consts.Rename.DataFile);
		public static IReadOnlyDictionary<Quarter, DateOnly> MapQuartersToFiledDates(string company, Quarter[] quarters) => FiledDateMapper.Map(company, quarters);
	}

	public static class Rows
	{
		public static class Download
		{
			public static StringyRowSet LoadStringy(string? quarter = null) => StringyRowsLoader.Load(GetZipFiles(quarter), LineMethod.OriginalEscaping);
			public static IEnumerable<T> LoadStringy<T>(string? quarter = null) where T : IStringyRow<T> => StringyRowsLoader.Load<T>(GetZipFiles(quarter), LineMethod.OriginalEscaping);

			static string[] GetZipFiles(string? quarter) =>
				quarter switch
				{
					not null => [Consts.Download.QuarterZipFile(quarter)],
					null => Consts.Download.GetAllZipFiles(),
				};
		}

		public static class Clean
		{
			public static StringyRowSet LoadStringy(string? quarter = null) => StringyRowsLoader.Load(GetZipFiles(quarter), LineMethod.ReplaceTabChar);
			public static IEnumerable<T> LoadStringy<T>(string? quarter = null) where T : IStringyRow<T> => StringyRowsLoader.Load<T>(GetZipFiles(quarter), LineMethod.ReplaceTabChar);

			public static RowSet Load(string? quarter = null) => RowsLoader.Load(GetZipFiles(quarter));
			public static IEnumerable<T> Load<T>(string? quarter = null) where T : IRow<T> => RowsLoader.Load<T>(GetZipFiles(quarter));

			static string[] GetZipFiles(string? quarter) =>
				quarter switch
				{
					not null => [Consts.Clean.QuarterZipFile(quarter)],
					null => Consts.Clean.GetAllZipFiles(),
				};
		}

		public static class Group
		{
			public static RowSet Load(string? company = null) => RowsLoader.Load(GetZipFiles(company));
			public static IEnumerable<T> Load<T>(string? company = null) where T : IRow<T> => RowsLoader.Load<T>(GetZipFiles(company));

			static string[] GetZipFiles(string? company) =>
				company switch
				{
					not null => [Consts.Group.CompanyZipFile(company)],
					null => Consts.Group.GetAllCompanyZipFiles(),
				};
		}
	}
}