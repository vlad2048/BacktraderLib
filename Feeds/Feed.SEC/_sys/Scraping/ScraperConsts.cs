using Microsoft.Playwright;

namespace Feed.SEC._sys.Scraping;

static class ScraperConsts
{
	public static readonly LocatorClickOptions clickOpt = new() { Timeout = 3000 };
	public static readonly LocatorFillOptions fillOpt = new() { Timeout = 1000 };
	public static readonly LocatorInnerTextOptions locInnerTextOpt = new() { Timeout = 1000 };
	public static readonly LocatorTextContentOptions locTextContentOpt = new() { Timeout = 1000 };
	public static readonly LocatorWaitForOptions locWaitOpt = new() { Timeout = 1000 };
}