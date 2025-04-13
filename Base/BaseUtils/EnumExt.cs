using LINQPad;


#pragma warning disable CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.

namespace BaseUtils;

public static class EnumExt
{
	public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
	{
		foreach (var elt in source)
			action(elt);
	}

	public static object[] ToObjects<T>(this T[] xs) => [..xs.OfType<object>()];

	public static HashSet<U> ToHashSet<T, U>(this IEnumerable<T> source, Func<T, U> fun) => source.Select(fun).ToHashSet();
	//public static Queue<T> ToQueue<T>(this IEnumerable<T> source) => new(source);

	public static T[] WhereA<T>(this IEnumerable<T> source, Func<T, bool> fun) => source.Where(fun).ToArray();
	public static U[] SelectA<T, U>(this IEnumerable<T> source, Func<T, U> fun) => source.Select(fun).ToArray();
	public static U[] SelectA<T, U>(this IEnumerable<T> source, Func<T, int, U> fun) => source.Select(fun).ToArray();
	public static T[] SelectManyA<T>(this IEnumerable<IEnumerable<T>> source) => source.SelectMany(e => e).ToArray();
	public static U[] SelectManyA<T, U>(this IEnumerable<T> source, Func<T, IEnumerable<U>> fun) => source.SelectMany(fun).ToArray();
	public static U[] SelectManyA<T, U>(this IEnumerable<T> source, Func<T, int, IEnumerable<U>> fun) => source.SelectMany(fun).ToArray();
	//public static T[] ConcatA<T>(this IEnumerable<T> first, IEnumerable<T> second) => first.Concat(second).ToArray();
	public static T[] ConcatDistinctA<T>(this IEnumerable<T> first, IEnumerable<T> second) => first.Concat(second).Distinct().ToArray();
	public static U[] SelectDistinctA<T, U>(this IEnumerable<T> source, Func<T, U> fun) => source.Select(fun).Distinct().ToArray();
	public static bool IsSame<T>(this IReadOnlyCollection<T> xs, IReadOnlyCollection<T> ys) => xs.Count == ys.Count && xs.Zip(ys).All(t => Equals(t.First, t.Second));

	public static int IdxOf<T>(this IEnumerable<T> source, T elt)
	{
		var idx = 0;
		foreach (var item in source)
		{
			if (item!.Equals(elt))
				return idx;
			idx++;
		}
		return -1;
	}

	public static IReadOnlyDictionary<K, V> ToOrderedDictionary<T, K, V>(this IEnumerable<T> source, Func<T, K> keyFun, Func<T, V> valFun) => new OrderedDictionary<K, V>(source.Select(e => new KeyValuePair<K, V>(keyFun(e), valFun(e))));

	public static SortedDictionary<K, V> ToSortedDictionary<K, V>(this IEnumerable<KeyValuePair<K, V>> source) where K : notnull => new(source.ToDictionary());

	public static T[] Shuffle<T>(this IEnumerable<T> source, int? seed)
	{
		var rnd = seed switch
		{
			not null => new Random(seed.Value),
			null => new Random((int)DateTime.Now.Ticks)
		};
		var array = source.ToArray();
		var n = array.Length;
		for (var i = 0; i < n - 1; i++)
		{
			var r = i + rnd.Next(n - i);
			(array[r], array[i]) = (array[i], array[r]);
		}
		return array;
	}

	public static T[] EnsureUniqueBy<T, K>(this IEnumerable<T> source, Func<T, K> keyFun, string err) where K : notnull
	{
		var arrAll = source.ToArray();
		var arrUniqLng = arrAll.DistinctBy(keyFun).Count();
		if (arrUniqLng != arrAll.Length)
		{
			var errMsg = $"[{err}] {typeof(T).Name} should be unique by {typeof(K).Name}";
			arrAll
				.GroupBy(keyFun)
				.Select(e => e.ToArray())
				.OrderByDescending(e => e.Length)
				.Where(e => e.Length > 1)
				.Select(e => e.Take(10))
				.Take(10)
				.Dump(errMsg);
			throw new ArgumentException(errMsg);
		}
		return arrAll;
	}
}