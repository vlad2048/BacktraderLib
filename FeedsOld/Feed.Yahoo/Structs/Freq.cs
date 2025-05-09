using System.Text.Json.Serialization;

namespace Feed.Yahoo;

public enum Freq
{
	[JsonStringEnumMemberName("1m")] Min = 0,
	[JsonStringEnumMemberName("2m")] Min2 = 1,
	[JsonStringEnumMemberName("5m")] Min5 = 2,
	[JsonStringEnumMemberName("15m")] Min15 = 3,
	[JsonStringEnumMemberName("30m")] Min30 = 4,
	//Min45 = 5,
	[JsonStringEnumMemberName("90m")] Min90 = 6,
	[JsonStringEnumMemberName("1h")] Hour = 7,
	//Hour2 = 8,
	//Hour4 = 9,
	[JsonStringEnumMemberName("1d")] Day = 10,
	[JsonStringEnumMemberName("5d")] Day5 = 11,
	[JsonStringEnumMemberName("1w")] Week = 12,
	[JsonStringEnumMemberName("1mo")] Month = 13,
	[JsonStringEnumMemberName("3mo")] Month3 = 14,
}