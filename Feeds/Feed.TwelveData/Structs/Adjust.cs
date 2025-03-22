using System.Text.Json.Serialization;

namespace Feed.TwelveData;

public enum Adjust
{
	[JsonStringEnumMemberName("all")] All = 0,
	[JsonStringEnumMemberName("splits")] Splits = 1,
	[JsonStringEnumMemberName("dividends")] Dividends = 2,
	[JsonStringEnumMemberName("none")] None = 3,
}