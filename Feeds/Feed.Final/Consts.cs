using FeedUtils;

namespace Feed.Final;

static class Consts
{
	static string RootFolder => API.Cfg.RootFolder;

	
	public static class Symbology
	{
		static readonly string Folder = Path.Combine(RootFolder, "symbology").CreateFolderIFN();

		public static string Download_Trading212Data_File => Path.Combine(Folder, "download-trading212.json");
		public static string Download_TwelveDataSymbols_File => Path.Combine(Folder, "download-twelvedata.json");
		public static string Download_Mics_File => Path.Combine(Folder, "download-mics.json");

		public static string Download_File => Path.Combine(Folder, "download.json");

		public static string Symbols_File => Path.Combine(Folder, "symbols.json");
	}



	public static class SECCompanyHistory
	{
		static readonly string Folder = Path.Combine(RootFolder, "fundamental", "sec-compiled").CreateFolderIFN();

		public static string Summary_File => Path.Combine(Folder, "summary.json");
	}
}