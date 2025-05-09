using BaseUtils;
using Feed.TwelveData._sys.Structs;

namespace Feed.TwelveData._sys;

static class TwelveDataBarExtractor
{
	public static TwelveDataBar[] ExtractBars(this TimeSeriesResponse result) =>
		result.Values
			.SelectA(e => new TwelveDataBar(
				e.Datetime,
				e.Open,
				e.Close,
				e.High,
				e.Low,
				e.Volume
			));
}