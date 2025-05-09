using BaseUtils;
using Frames;

namespace Feed.NoPrefs;

public sealed record Yields(
	DateTime[] Index,
	SortedDictionary<int, decimal?[]> Map
)
{
	public bool IsEmpty => (Index.Length, Map.Count) switch
	{
		(0, 0) => true,
		(> 0, > 0) => false,
		_ => throw new ArgumentException("Invalid Yields"),
	};
}



static class YieldsUtils
{
	public static Yields Merge(Yields prev, Yields next)
	{
		if (prev.IsEmpty) return next;
		if (next.IsEmpty) return prev;
		if (!prev.Map.Keys.IsSame(next.Map.Keys)) throw new ArgumentException("Incompatible Yields keys");
		var keys = prev.Map.Keys.ToArray();
		var index = prev.Index.Concat(next.Index).Distinct().OrderBy(e => e).ToArray();
		var map = new SortedDictionary<int, decimal?[]>();
		foreach (var key in keys)
		{
			var prevMap = prev.Index.Index().ToDictionary(t => t.Item, t => prev.Map[key][t.Index]);
			var nextMap = next.Index.Index().ToDictionary(t => t.Item, t => next.Map[key][t.Index]);
			var arr = new decimal?[index.Length];
			for (var i = 0; i < index.Length; i++)
			{
				var t = index[i];
				var val = nextMap.ContainsKey(t) switch
				{
					true => nextMap[t],
					false => prevMap[t],
				};
				arr[i] = val;
			}

			map[key] = arr;
		}
		return new Yields(index, map);
	}

	public static Yields SanityCheck(this Yields yields)
	{
		if (yields.Index.Length == 0 ^ yields.Map.Count == 0) throw new ArgumentException("Invalid Yields");
		var uniq = yields.Index.Distinct().Count();
		var n = yields.Index.Length;
		if (uniq != n) throw new ArgumentException("Non unique dates");
		if (yields.Map.Values.Any(e => e.Length != n)) throw new ArgumentException("Inconsistent Yields");
		return yields;
	}


	public static Frame<string, int> ToFrame(this Yields yields) =>
		Frame.Make(
			"Yields",
			yields.Index,
			yields.Map.SelectA(kv => (kv.Key, kv.Value.ToDouble()))
		);

	static double[] ToDouble(this decimal?[] arr) => arr.Select(e => e.HasValue ? (double)e : double.NaN).ToArray();
}