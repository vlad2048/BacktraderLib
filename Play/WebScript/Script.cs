global using static WebScript.StaticsAccessor;
using LINQPad;
using Microsoft.Playwright;

namespace WebScript;


static class StaticsContainer
{
	public static IPage? Page { get; set; }
	public static ITracing? Tracing { get; set; }
	public static DumpContainer? LogDC { get; set; }
	public static CancellationToken? Cancel { get; set; }
}

static class StaticsAccessor
{
	public static IPage Page => StaticsContainer.Page ?? throw new ArgumentException("StaticsContainer.Page not initialized");
	public static ITracing Tracing => StaticsContainer.Tracing ?? throw new ArgumentException("StaticsContainer.Tracing not initialized");
	public static void LogAppend(object obj)
	{
		if (StaticsContainer.LogDC != null)
			StaticsContainer.LogDC.AppendContent(obj);
		else
			obj.Dump();
	}
	public static CancellationToken Cancel => StaticsContainer.Cancel ?? throw new ArgumentException("StaticsContainer.Cancel not initialized");
	public static void CancelCheck() => Cancel.ThrowIfCancellationRequested();
}


static class Script
{
	public static async Task Main(IPage page, ITracing? tracing, DumpContainer logDC, CancellationToken cancel)
	{
		(StaticsContainer.Page, StaticsContainer.Tracing, StaticsContainer.LogDC, StaticsContainer.Cancel) = (page, tracing, logDC, cancel);

		LogAppend("Start");

		if (await Page.GetByTestId("layout-component").GetByTestId("search-bar").CountAsync() == 0)
		{
			LogAppend("    [Search] Click on search");
			await Page.GetByTestId("app-header-search-button").ClickAsync();
		}
		else
		{
			LogAppend("    [Search] Already in search");
		}


		LogAppend("Done");
	}
}