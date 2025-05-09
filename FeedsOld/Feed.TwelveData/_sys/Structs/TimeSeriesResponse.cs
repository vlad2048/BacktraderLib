namespace Feed.TwelveData._sys.Structs;

sealed record TimeSeriesResponse(
	Meta Meta,
	TwelveDataBar[] Values
);


static class TimeSeriesResponseUtils
{
	public static TimeSeriesResponse Combine(IReadOnlyList<TimeSeriesResponse> list)
	{
		if (list.Count == 0) throw new ArgumentException("Impossible. Cannot be empty");

		var set = new HashSet<DateTime>();
		var allValues = new List<TwelveDataBar>();
		for (var i = list.Count - 1; i >= 0; i--)
		{
			var elt = list[i];
			allValues.AddRange(elt.Values.Where(val => set.Add(val.Datetime)));
		}

		var first = list[0];
		var final = first with { Values = [..allValues] };
		return final;
	}

	public static TimeSeriesResponse Validate(this TimeSeriesResponse res)
	{
		// Check the bars are not empty
		if (res.Values.Length == 0) throw new ArgumentException($"No bars found for {res.Meta.Symbol}");

		// Check all Datetimes are unique
		var set = new HashSet<DateTime>();
		foreach (var val in res.Values)
			if (!set.Add(val.Datetime))
				throw new ArgumentException($"Datetime is not unique: {val.Datetime} for {res.Meta.Symbol}");

		// Check all Datetimes are increasing
		for (var i = 0; i < res.Values.Length - 1; i++)
		{
			var datePrev = res.Values[i].Datetime;
			var dateNext = res.Values[i + 1].Datetime;
			if (datePrev >= dateNext) throw new ArgumentException($"Datetimes are not increasing {datePrev} >= {dateNext} for {res.Meta.Symbol}");
		}

		return res;
	}
}