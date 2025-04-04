using BaseUtils;
using Microsoft.Playwright;

namespace WebScript;

static class Consts
{
	//public static readonly Quarter MinScrapeQuarter = Quarter.MinValue;

	public static readonly Quarter MinScrapeQuarter = new(2024, QNum.Q1);

	public static readonly LocatorClickOptions Click_MoreFinancialsDelay = new()
	{
		Timeout = 10000,
	};

	public static readonly LocatorClickOptions Click_QuarterlyDelay = new()
	{
		Timeout = 500,
	};
}