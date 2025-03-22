using LINQPad;

namespace BaseUtils;

public static class DateAligner
{
	public static IReadOnlyDictionary<DateTime, int> Align(SortedSet<DateTime>[] xss)
	{
		Verify_SingleDatePerDay(xss);

		return xss
			.SelectMany(e => e)
			.Distinct()
			.OrderBy(e => e)
			.Select((e, i) => (e, i))
			.ToOrderedDictionary(t => t.e, t => t.i);
	}



	static void Verify_SingleDatePerDay(IEnumerable<SortedSet<DateTime>> xss)
	{
		var xs = xss.SelectMany(e => e).Distinct().ToArray();
		var cnt = xs.GroupBy(DateOnly.FromDateTime).Count();
		if (xs.Length != cnt)
		{
			xs.GroupBy(DateOnly.FromDateTime)
				.Where(grp => grp.Count() > 1)
				.SelectA(grp => new
				{
					Date = grp.Key,
					DateTimes = grp.ToArray(),
				}).Dump();

			throw new ArgumentException("Multiple DateTimes on the same day");
		}
	}
}
