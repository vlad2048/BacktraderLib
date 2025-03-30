using System.Net;
using HtmlAgilityPack;

namespace Feed.SEC._sys.Utils;


public sealed record ScriptHeader(string Name, string[] Values);
public sealed record ScriptCookie(string Name, string Value, string? Path, string? Domain);
public sealed record ScriptData(
	ScriptHeader[] Headers,
	ScriptCookie[] Cookies
);



static class HttpUtils
{
	public static HtmlDocument GetHtmlPage(string url, ScriptData data)
	{
		SetCookies(url, data.Cookies);
		var request = new HttpRequestMessage(HttpMethod.Get, url).AddHeaders(data.Headers);
		var response = client.Send(request);
		if (!response.IsSuccessStatusCode)
			throw new ArgumentException($"Failed to get '{url}' with StatusCode: {response.StatusCode}");
		var html = response.Content.ReadAsStringAsync().Result;
		var htmlDoc = new HtmlDocument();
		htmlDoc.LoadHtml(html);
		return htmlDoc;
	}


	public static byte[] DownloadFile(string url, ScriptData data)
	{
		SetCookies(url, data.Cookies);
		var request = new HttpRequestMessage(HttpMethod.Get, url).AddHeaders(data.Headers);
		var response = client.Send(request);
		if (!response.IsSuccessStatusCode)
			throw new ArgumentException($"Failed to get '{url}' with StatusCode: {response.StatusCode}");
		return response.Content.ReadAsByteArrayAsync().Result;
	}



	static readonly CookieContainer cookieContainer = new();
	static readonly HttpClient client = new(new HttpClientHandler
	{
		CookieContainer = cookieContainer,
		AutomaticDecompression = DecompressionMethods.All,
	});

	static void SetCookies(string url, ScriptCookie[] cookies)
	{
		var urlBase = GetUrlBase(url);
		cookieContainer.GetCookies(urlBase).Clear();
		foreach (var cookie in cookies)
			cookieContainer.Add(urlBase, new Cookie(cookie.Name, cookie.Value, cookie.Path, cookie.Domain));
	}

	static HttpRequestMessage AddHeaders(this HttpRequestMessage req, ScriptHeader[] headers)
	{
		foreach (var header in headers)
			req.Headers.Add(header.Name, header.Values);
		return req;
	}

	static Uri GetUrlBase(string urlStr)
	{
		var url = new Uri(urlStr);
		var urlPath = url.AbsolutePath;
		var idx = urlStr.IndexOf(urlPath, StringComparison.Ordinal);
		if (idx == -1) throw new ArgumentException("Failed to split URL");
		var urlBase = urlStr[..idx];
		var uriBase = new Uri(urlBase);
		return uriBase;
	}
}