using Microsoft.Playwright;

namespace WebScript._sys.Utils;

static class PlaywrightExt
{
	public static async Task<IRequest> FillAndWaitFor(this ILocator loc, string text, Func<IRequest, bool> predicate) =>
		await Page.RunAndWaitForRequestFinishedAsync(
			async () => await loc.FillAsync(text),
			new PageRunAndWaitForRequestFinishedOptions
			{
				Predicate = predicate,
			}
		);


	public static async Task<IRequest> ClickAndWaitFor(this ILocator loc, Func<IRequest, bool> predicate, LocatorClickOptions? clickOptions = null) =>
		await Page.RunAndWaitForRequestFinishedAsync(
			async () => await loc.ClickAsync(clickOptions),
			new PageRunAndWaitForRequestFinishedOptions
			{
				Predicate = predicate,
			}
		);
}