using BaseUtils;
using LINQPad.FSharpExtensions;

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
		var done = new HashSet<string>();
		while (queue.TryDequeue(out var cur))
		{
			if (!File.Exists(Consts.Group.CompanyZipFile(cur))) continue;
			list.Add(cur);
			done.Add(cur);
			var formers = API.Rows.Group.Load<SubRow>(cur)
				.Where(e => e.Former != null)
				.Select(e => e.Former!)
				.Distinct();
			foreach (var former in formers)
			{
				if (!done.Contains(former))
					queue.Enqueue(former);
			}
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
				//g => g.Select(e => e.Filed).Distinct().SingleOrDefaultDiag(g)
				g => g.Select(e => e.Filed).Distinct().MinBy(e => e)
			);

	static T SingleOrDefaultDiag<T, S>(this IEnumerable<T> source, IEnumerable<S> orig)
	{
		var arr = source.ToArray();
		if (arr.Length == 1)
			return arr[0];
		if (arr.Length == 0)
			throw new ArgumentException("Impossible (SingleOrDefaultDiag)");

		orig.Dump();
		throw new ArgumentException("SingleOrDefaultDiag check failed");
	}

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