/*
using Feed.Trading212._sys;
using Microsoft.Playwright;

namespace Feed.Trading212;

public static class Script
{
	public static async Task Run(IPage page, Action<object> log)
	{
		log("Script.Run()");

		var res = await Scraper.Search("TSLA");

		log(res.Select(e => e.Item));
	}
}
*/