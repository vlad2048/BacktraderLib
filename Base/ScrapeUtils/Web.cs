using Microsoft.Playwright;

namespace ScrapeUtils;

public static class Web
{
	static IBrowserContext? browserCtx;

	internal static void Init() => browserCtx?.DisposeAsync();


	public static ITracing Tracing => (browserCtx ?? throw new ArgumentException("Call Web.Open() first")).Tracing;

	public static async Task<IPage> Open(string url)
	{
		var playwright = await Playwright.CreateAsync();
		browserCtx = await playwright.Chromium.LaunchPersistentContextAsync(
			Consts.ChromeUserDataFolder,
			new BrowserTypeLaunchPersistentContextOptions
			{
				Headless = false,
			}
		);

		browserCtx.SetDefaultTimeout(3000);
		browserCtx.SetDefaultNavigationTimeout(30000);

		IPage page;
		if (browserCtx.Pages.Count > 0)
			page = browserCtx.Pages[0];
		else
			page = await browserCtx.NewPageAsync();

		await page.GotoAsync(url);

		return page;
	}
}