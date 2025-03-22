using System.Text.Json.Serialization;

namespace Feed.TwelveData;

public enum Freq
{
	[JsonStringEnumMemberName("1min")] Min = 0,
	//Min2 = 1,
	[JsonStringEnumMemberName("5min")] Min5 = 2,
	[JsonStringEnumMemberName("15min")] Min15 = 3,
	[JsonStringEnumMemberName("30min")] Min30 = 4,
	[JsonStringEnumMemberName("45min")] Min45 = 5,
	//Min90 = 6,
	[JsonStringEnumMemberName("1h")] Hour = 7,
	[JsonStringEnumMemberName("2h")] Hour2 = 8,
	[JsonStringEnumMemberName("4h")] Hour4 = 9,
	[JsonStringEnumMemberName("1day")] Day = 10,
	//Day5 = 11,
	[JsonStringEnumMemberName("1week")] Week = 12,
	[JsonStringEnumMemberName("1month")] Month = 13,
    //Month3 = 14,
}