using System.Diagnostics;
using BaseUtils;
using Feed.SEC._sys.Utils;
using HtmlAgilityPack;

namespace Feed.SEC._sys;

static class _1_Download
{
	public static void Run()
	{
		var Log = Logger.Make(LogCategory._1_Download);
		if (!HasEnoughTimePassedToCheckAgain(Log))
			return;

		var htmlDoc = HttpUtils.GetHtmlPage(linkPageUrl, linkPageRequestData);
		var quartersNext = FindZipLinksInHtml(htmlDoc);
		var quartersPrev = Consts.Download.GetAllQuarters();
		var quartersNew = quartersNext.ExceptA(quartersPrev, link2NameComparer);

		quartersNew.Loop(Log, 1, "Download", x => Consts.Download.QuarterZipFile(x).FmtArchFile(), quarterNew =>
		{
			var bytes = HttpUtils.DownloadFile(quarterNew, zipLinkRequestData);
			var quarter = Path.GetFileNameWithoutExtension(quarterNew);
			var zipFile = Consts.Download.QuarterZipFile(quarter);
			File.WriteAllBytes(zipFile, bytes);
			Log($"bytes:{bytes.Length}");
		});

		DateTime.Now.Save(Consts.Download.LastCheckFile);
	}

	static readonly IEqualityComparer<string> link2NameComparer = EqualityUtils.Make<string, string>(Path.GetFileNameWithoutExtension);


	const string linkPageUrl = "https://www.sec.gov/data-research/sec-markets-data/financial-statement-data-sets";
	static readonly ScriptData linkPageRequestData = new(
		[
			new ScriptHeader("Accept", ["application/signed-exchange"]),
			new ScriptHeader("Accept-Encoding", ["gzip, deflate, br, zstd"]),
			new ScriptHeader("sec-fetch-site", ["none"]),
			new ScriptHeader("User-Agent", ["Mozilla/5.0 (Windows NT; Windows NT 10.0; en-GB) WindowsPowerShell/5.1.19041.5607"]),
		],
		[
		]
	);
	const string zipLinkPrefix = "https://www.sec.gov";

	static readonly ScriptData zipLinkRequestData = new(
		[
			new ScriptHeader("Accept", ["application/signed-exchange"]),
			new ScriptHeader("Accept-Encoding", ["gzip, deflate, br, zstd"]),
			new ScriptHeader("User-Agent", ["Mozilla/5.0 (Windows NT; Windows NT 10.0; en-GB) WindowsPowerShell/5.1.19041.5607"]),
		],
		[
		]
	);


	static bool HasEnoughTimePassedToCheckAgain(Action<string> Log)
	{
		var timePrev = JsonUtils.LoadOr(Consts.Download.LastCheckFile, DateTime.MinValue);
		var timeNext = DateTime.Now;
		var res = timeNext - timePrev >= Consts.Download.CheckDelay;
		Log($"lastChecked:{timePrev} => check:{$"{res}".ToUpperInvariant()}");
		return res;
	}


	static string[] FindZipLinksInHtml(HtmlDocument htmlDoc)
	{
		var nodes = htmlDoc.DocumentNode.SelectNodes("//a[contains(@href, 'financial-statement-data-sets') and contains(@href, '.zip')]");
		var links = new List<string>();
		foreach (var node in nodes)
		{
			var hrefValue = node.GetAttributeValue("href", string.Empty);
			if (!string.IsNullOrEmpty(hrefValue))
			{
				var link = $"{zipLinkPrefix}{hrefValue}";
				links.Add(link);
			}
		}
		return [..links];
	}
}