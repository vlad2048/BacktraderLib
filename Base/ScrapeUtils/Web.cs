using LINQPad;
using Microsoft.Playwright;

namespace ScrapeUtils;

public sealed class WebOpt
{
	public bool Headless { get; init; } = false;
	public float DefaultTimeoutMS { get; init; } = 3000;
	public float DefaultNavigationTimeoutMS { get; init; } = 30000;
}

public sealed class Web : IDisposable
{
	readonly string url;
	readonly WebOpt opt;
	IBrowserContext? browserCtx;
	IPage? page;

	public void Dispose() => browserCtx?.DisposeAsync();

	public IPage Page => page ?? throw new ArgumentException("Call Web.EnsurePageIsReady() first");
	public DumpContainer Log { get; }
	public CancellationToken? CancelToken { get; set; }
	public FullStatsKeeper? Stats { get; set; }


	public Web(
		string url,
		DumpContainer logDC,
		WebOpt? opt = null
	)
	{
		(this.url, Log, this.opt) = (url, logDC, opt ?? new WebOpt());
	}


	public async Task InitIFN()
	{
		if (page != null ^ browserCtx != null) throw new ArgumentException("[Web] Inconsistent state (1)");
		if (page != null) return;
		(browserCtx, page) = await WebCreateUtils.Create(url, opt);
	}

	public async Task Reset()
	{
		if (page != null ^ browserCtx != null) throw new ArgumentException("[Web] Inconsistent state (2)");
		if (browserCtx != null)
			await browserCtx.DisposeAsync();
		(browserCtx, page) = await WebCreateUtils.Create(url, opt);
	}

	public async Task SetOffline(bool isOffline)
	{
		await InitIFN();
		try
		{
			await browserCtx!.SetOfflineAsync(isOffline);
		}
		catch (Exception ex)
		{
			ex.Dump();
		}
	}


	internal async Task TryRun(Func<Task> action, string name, RetryPolicy? policy) =>
		await Retrier.Run(
			action,
			name,
			policy ?? RetryPolicy.Default,
			CancelToken ?? CancellationToken.None,
			Stats
		);

	internal async Task<T> TryReturn<T>(Func<Task<T>> action, string name, RetryPolicy? policy) where T : class =>
		await Retrier.Return(
			action,
			name,
			policy ?? RetryPolicy.Default,
			CancelToken ?? CancellationToken.None,
			Stats
		);
}



file static class WebCreateUtils
{
	public static async Task<(IBrowserContext, IPage)> Create(string url, WebOpt opt)
	{
		var playwright = await Playwright.CreateAsync();
		var browserCtx = await playwright.Chromium.LaunchPersistentContextAsync(
			Consts.ChromiumUserDataDir,
			new BrowserTypeLaunchPersistentContextOptions
			{
				Channel = opt.Headless switch
				{
					false => null,
					true => "chromium",
				},
				Headless = opt.Headless,
				Args = [
					$"--profile-directory={Consts.ChromiumProfileDirectory}",
				],
				UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/134.0.0.0 Safari/537.36",
			}
		);

		browserCtx.SetDefaultTimeout(opt.DefaultTimeoutMS);
		browserCtx.SetDefaultNavigationTimeout(opt.DefaultNavigationTimeoutMS);

		IPage page;
		if (browserCtx.Pages.Count > 0)
			page = browserCtx.Pages[0];
		else
			page = await browserCtx.NewPageAsync();

		await page.GotoAsync(url);

		return (browserCtx, page);
	}
}
