namespace _sys.Logging.Utils;

static class TreeEnumExt
{
	public static Dictionary<L, V> MapKey<K, L, V>(this Dictionary<K, V> dict, Func<K, L> mapFun) where K : notnull where L : notnull
	{
		var dictRes = new Dictionary<L, V>();
		foreach (var (k, v) in dict)
			dictRes[mapFun(k)] = v;
		return dictRes;
	}

	public static int SumOrZero(this IEnumerable<int> source)
	{
		var sum = 0;
		foreach (var elt in source)
			sum += elt;
		return sum;
	}

	public static int MaxOrZero(this IEnumerable<int> source)
	{
		var max = 0;
		foreach (var elt in source)
			if (elt > max)
				max = elt;
		return max;
	}
}