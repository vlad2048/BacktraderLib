using BaseUtils;

namespace Feed.SEC._sys.Logic;

static class FiledDateMapper
{
	public static IReadOnlyDictionary<Quarter, DateOnly> Map(string company, Quarter[] quarters) =>
		GetHistNames(company)
			.Select(MapHistName)
			.Merge()
			.ToSortedDictionary();
			//.ExtrapolateMissingQuarters(quarters);


	static string[] GetHistNames(string company)
	{
		var list = new List<string>();
		var queue = new Queue<string>();
		queue.Enqueue(company);
		while (queue.TryDequeue(out var cur))
		{
			list.Add(cur);
			var formers = API.Rows.Group.Load<SubRow>(cur)
				.Where(e => e.Former != null)
				.Select(e => e.Former!)
				.Distinct();
			foreach (var former in formers)
				queue.Enqueue(former);
		}

		return [.. list];
	}


	static IReadOnlyDictionary<Quarter, DateOnly> MapHistName(string histName) =>
		API.Rows.Group.Load<SubRow>(histName)
			.Where(e => e.Form is "10-Q" or "10-K")
			.GroupBy(e => e.Quarter)
			.OrderBy(g => g.Key)
			.ToDictionary(
				g => g.Key,
				g => g.Select(e => e.Filed).Distinct().SingleOrDefault()
			);


	static IReadOnlyDictionary<Quarter, DateOnly> Merge(this IEnumerable<IReadOnlyDictionary<Quarter, DateOnly>> source)
	{
		var map = new Dictionary<Quarter, DateOnly>();
		foreach (var item in source.Reverse())
		foreach (var (quarter, filed) in item)
			map[quarter] = filed;
		return map;
	}


	static IReadOnlyDictionary<Quarter, DateOnly> ExtrapolateMissingQuarters(this IReadOnlyDictionary<Quarter, DateOnly> mapSrc, Quarter[] quarters)
	{
		var mapDst = mapSrc.ToSortedDictionary();
		var quartersMissing = quarters.WhereA(e => !mapDst.ContainsKey(e));

		foreach (var quarterMissing in quartersMissing)
		{
			var matching = mapDst.WhereA(kv => kv.Key.Q == quarterMissing.Q);
			if (matching.Length > 0)
			{
				var closest = matching.MinBy(kv => quarterMissing.Distance(kv.Key));
				mapDst[quarterMissing] = closest.Value.ChangeYear(quarterMissing.Year);
			}
		}

		return mapDst;
	}


	static DateOnly ChangeYear(this DateOnly date, int year)
	{
		var month = date.Month;
		var dayMax = DateTime.DaysInMonth(year, month);
		var day = Math.Min(date.Day, dayMax);
		return new DateOnly(year, month, day);
	}
}