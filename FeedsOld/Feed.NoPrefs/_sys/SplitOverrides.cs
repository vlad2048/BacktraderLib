namespace Feed.NoPrefs._sys;

static class SplitOverrides
{
	static readonly Dictionary<string, PriceSplit[]> map = new()
	{
		["GOOG"] = [
			Split(T(2014, 3, 27), 2002, 1000),
			Split(T(2015, 4, 27), 10027455, 10000000),
			Split(T(2022, 7, 18), 20, 1),
		],
	};

	public static PriceSplit[]? Get(string symbol) =>
		map.TryGetValue(symbol, out var splits) switch
		{
			true => splits,
			false => null,
		};


	static PriceSplit Split(DateTime exDate, decimal toFactor, decimal forFactor) => new(exDate, toFactor, forFactor);
	static DateTime T(int year, int month, int day) => new(year, month, day);
}