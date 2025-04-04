using Feed.Trading212._sys;
using Feed.Trading212._sys.Utils;

namespace Feed.Trading212;

public static class API
{
	public static async Task Scrape(SymbolDef[] symbols) => await ScraperRunner.Scrape(symbols);

	public static SymbolData Load(string secCompany) => JsonUtils.Load<SymbolData>(Consts.Data.CompanyJsonFile(secCompany));


	/*public static async Task EnableScript()
	{
		await InitPageIFN();

		var projName = Path.GetFileName(Consts.ScriptProjFolder);
		var assFile = Path.Combine(Consts.ScriptProjFolder, "bin", "Debug", "net9.0", $"{projName}.dll");

		FileWatcher.Watch(assFile)
			.Subscribe(async _ =>
			{
				DCLog.ClearContent();
				await ScriptRunner.Run(Consts.ScriptProjFolder, Page, Log);
			}).D(FeedTrading212Setup.D);
	}*/
}
