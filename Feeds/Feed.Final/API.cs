using Feed.Final._sys;
using Feed.Final._sys.CompanyHistoryPartitioning.GraphPrinting;
using Feed.Final._sys.Structs;
using Feed.Final._sys.UtilsSteppers;
using Feed.Symbology;
using FeedUtils;

namespace Feed.Final;

public static class API
{
	static Cfg? cfg { get; set; }
	internal static Cfg Cfg => cfg ?? throw new ArgumentException("Call Feed.Final.API.Init() first");



	public static void Init(string trading212ApiKey, string twelveDataApiKey, string? rootFolder = null)
	{
		rootFolder ??= FileUtils.GetProjectRootFolder("Feed.Final");
		Feed.Symbology.API.Init(trading212ApiKey, twelveDataApiKey);
		SEC.API.Init(Path.Combine(rootFolder, "fundamental", "sec").CreateFolderIFN());
		cfg = new Cfg(rootFolder);
		GraphInit.Init();
		StepperInit.Init();
	}



	public static class Symbology
	{
		public static Trading212SymbolData Download_Trading212Data => download_Trading212Data.Value;
		public static TwelveDataSymbol[] Download_TwelveDataSymbols => download_TwelveDataSymbols.Value;
		public static Mic[] Download_Mics => download_Mics.Value;

		public static SymbologyData DownloadData => new(Download_Trading212Data, Download_TwelveDataSymbols, Download_Mics);

		public static Symbol[] Symbols => symbols.Value;

		
		static readonly CacheFile<Trading212SymbolData> download_Trading212Data = new(
			Consts.Symbology.Download_Trading212Data_File,
			CacheStrat.TooOld(TimeSpan.FromDays(1)),
			Feed.Symbology.API.FetchTrading212Data
		);

		static readonly CacheFile<TwelveDataSymbol[]> download_TwelveDataSymbols = new(
			Consts.Symbology.Download_TwelveDataSymbols_File,
			CacheStrat.TooOld(TimeSpan.FromDays(1)),
			Feed.Symbology.API.FetchTwelveDataSymbols
		);

		static readonly CacheFile<Mic[]> download_Mics = new(
			Consts.Symbology.Download_Mics_File,
			CacheStrat.TooOld(TimeSpan.FromDays(1)),
			Feed.Symbology.API.FetchMics
		);


		static readonly CacheFile<Symbol[]> symbols = new(
			Consts.Symbology.Symbols_File,
			CacheStrat.FilesUpdated(
			[
				Consts.Symbology.Download_Trading212Data_File,
				Consts.Symbology.Download_TwelveDataSymbols_File,
				Consts.Symbology.Download_Mics_File,
			]),
			() =>
				DownloadData
					.Clean()
					.Check()
					.Match()
					.ToSymbols()
			);
	}



	public static class SECCompanyHistory
	{
		public static SecSummary Summary => summary.Value;

		static readonly CacheFile<SecSummary> summary = new(
			Consts.SECCompanyHistory.Summary_File,
			CacheStrat.FilesUpdated(SEC.API.Utils.GroupQuartersDoneFile),
			SecSummarizer.Summarize
		);
	}
}