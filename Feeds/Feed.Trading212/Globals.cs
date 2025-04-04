global using static Feed.Trading212.Globals;
global using QuarterSet = System.Collections.Generic.Dictionary<Feed.Trading212.ReportType, System.Collections.Generic.HashSet<BaseUtils.Quarter>>;
using Microsoft.Playwright;
using System.Reactive.Disposables;

//[assembly: InternalsVisibleTo("LINQPadQuery")]

namespace Feed.Trading212;

static class Globals
{
	public static IPage Page => FeedTrading212Setup.Page ?? throw new ArgumentException("Page not initialized");

	public static async Task Pause(double seconds) => await Task.Delay(TimeSpan.FromSeconds(seconds));
}

public static class FeedTrading212Setup
{
	internal static IPage? Page { get; set; }

	static CompositeDisposable? d;
	internal static CompositeDisposable D => d ?? throw new ArgumentException("You forgot to call FeedTrading212Setup.Init()");

	public static void Init()
	{
		d?.Dispose();
		d = new CompositeDisposable();
	}
}