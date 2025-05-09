namespace Feed.SEC;

public sealed record RowSet(
	NumRow[] Nums,
	PreRow[] Pres,
	SubRow[] Subs,
	TagRow[] Tags
)
{
	public static readonly RowSet Empty = new([], [], [], []);
	public bool IsEmpty => Nums.Length == 0 && Pres.Length == 0 && Subs.Length == 0 && Tags.Length == 0;
}




static class RowSetUtils
{
	public static RowSet Merge(this IEnumerable<RowSet> sets)
	{
		var setsArr = sets.ToArray();
		return new RowSet(
			setsArr.SelectMany(e => e.Nums).DistinctBy(e => e.Key).ToArray(),
			setsArr.SelectMany(e => e.Pres).DistinctBy(e => e.Key).ToArray(),
			setsArr.SelectMany(e => e.Subs).DistinctBy(e => e.Key).ToArray(),
			setsArr.SelectMany(e => e.Tags).DistinctBy(e => e.Key).ToArray()
		);
	}
}
