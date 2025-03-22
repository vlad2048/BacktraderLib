using OoplesFinance.YahooFinanceAPI.Models;
using System.Text.Json;
using FeedUtils;

namespace Feed.YahooOoples._sys;

static class JsonUtils
{
	static readonly JsonSerializerOptions jsonOpt = new()
	{
		WriteIndented = true,
		IndentCharacter = '\t',
		IndentSize = 1,
		Converters = {
			JsonConverterMaker.Make<DividendInfo, DividendInfoRec>(DividendInfoRec.ToRec, DividendInfoRec.FromRec),
			JsonConverterMaker.Make<SplitInfo, SplitInfoRec>(SplitInfoRec.ToRec, SplitInfoRec.FromRec),
		},
	};



	public static T Save<T>(this T obj, string file)
	{
		Path.GetDirectoryName(file)!.CreateFolderIFN();
		File.WriteAllText(file, obj.Ser());
		return obj;
	}

	public static T Load<T>(string file) => File.ReadAllText(file).Deser<T>();


	public static string Ser<T>(this T obj) => JsonSerializer.Serialize(obj, jsonOpt);

	public static T Deser<T>(this string json) => JsonSerializer.Deserialize<T>(json, jsonOpt) ?? throw new ArgumentException("JsonSerializer.Deserialize returned null");




	sealed record DividendInfoRec(DateTime Date, double? Amount)
	{
		public static DividendInfo FromRec(DividendInfoRec t) => new(new Dividends { Date = (int)t.Date.ToUnixTimestamp(), Amount = t.Amount });
		public static DividendInfoRec ToRec(DividendInfo e) => new(e.Date, e.Amount);
	}

	sealed record SplitInfoRec(DateTime? Date, long? Numerator, long? Denominator, string SplitRatio)
	{
		public static SplitInfo FromRec(SplitInfoRec t) => new(new Splits { Date = t.Date.HasValue ? t.Date!.Value.ToUnixTimestamp() : 0, Numerator = t.Numerator, Denominator = t.Denominator, SplitRatio = t.SplitRatio });
		public static SplitInfoRec ToRec(SplitInfo e) => new(e.Date, e.Denominator, e.Numerator, e.SplitRatio);
	}


	static long ToUnixTimestamp(this DateTime dateTime)
	{
		return new DateTimeOffset(dateTime).ToUnixTimeSeconds();
	}
	static DateTime FromUnixTimeStamp(this long unixTimestamp)
	{
		return DateTimeOffset.FromUnixTimeSeconds(unixTimestamp).DateTime;
	}
}