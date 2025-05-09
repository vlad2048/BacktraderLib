using System.Runtime.CompilerServices;
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
	public static T[] ConcatA<T>(this IEnumerable<T> first, IEnumerable<T> second) => first.Concat(second).ToArray();
	public static T[] ConcatDistinctA<T>(this IEnumerable<T> first, IEnumerable<T> second) => first.Concat(second).Distinct().ToArray();
	public static U[] SelectDistinctA<T, U>(this IEnumerable<T> source, Func<T, U> fun) => source.Select(fun).Distinct().ToArray();
	public static IEnumerable<T> WhereNot<T>(this IEnumerable<T> xs, Func<T, bool> fun) => xs.Where(e => !fun(e));
	public static bool IsSame<T>(this IReadOnlyCollection<T> xs, IReadOnlyCollection<T> ys) => xs.Count == ys.Count && xs.Zip(ys).All(t => Equals(t.First, t.Second));

	public static bool IsSameInAnyOrder<T>(this IEnumerable<T> xs, IEnumerable<T> ys)
	{
		var xsArr = xs.ToArray();
		var ysArr = ys.ToArray();
		var xsSet = xsArr.ToHashSet();
		var ysSet = ysArr.ToHashSet();
		if (xsSet.Count != xsArr.Length)
			return false;
		if (ysSet.Count != ysArr.Length)
			return false;
		if (xsSet.Count != ysSet.Count)
			return false;
		if (xsSet.Count(e => !ysSet.Contains(e)) > 0)
			return false;
		if (ysSet.Count(e => !xsSet.Contains(e)) > 0)
			return false;
		return true;
	}

	public static T[] UniquifyBy<T, K>(this IEnumerable<T> source, Func<T, K> keyFun) => source.GroupBy(keyFun).SelectA(e => e.First());

	public static T[] GetConflicts<T, K>(this IEnumerable<T> source, Func<T, K> fun) =>
		source
			.GroupBy(fun)
			.Select(e => e.ToArray())
			.OrderByDescending(e => e.Length)
			.Where(e => e.Length > 1)
			.SelectManyA();

	public static int IdxOf<T>(this IEnumerable<T> source, T elt, int? idxStart = null)
	{
		var idx = 0;
		foreach (var item in source)
		{
			var startReached = !idxStart.HasValue || idx >= idxStart.Value;
			if (startReached && item!.Equals(elt))
				return idx;
			idx++;
		}
		return -1;
	}

	public static void AddRange<T>(this HashSet<T> set, IEnumerable<T> items) => items.ForEach(e => set.Add(e));

	public static IReadOnlyDictionary<K, V> ToOrderedDictionary<T, K, V>(this IEnumerable<T> source, Func<T, K> keyFun, Func<T, V> valFun) => new OrderedDictionary<K, V>(source.Select(e => new KeyValuePair<K, V>(keyFun(e), valFun(e))));

	public static SortedDictionary<K, V> ToSortedDictionary<K, V>(this IEnumerable<KeyValuePair<K, V>> source) where K : notnull => new(source.ToDictionary());

	public static SortedDictionary<K2, V2> ToSortedDictionary<K1, K2, V1, V2>(this IEnumerable<KeyValuePair<K1, V1>> source, Func<K1, K2> keyFun, Func<V1, V2> valFun) where K1 : notnull where K2 : notnull => new(source.ToDictionary(e => keyFun(e.Key), e => valFun(e.Value)));

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


	public static IReadOnlyDictionary<K, T[]> GroupInDict<T, K>(this IEnumerable<T> source, Func<T, K> fun) where K : notnull => source.GroupBy(fun).ToDictionary(e => e.Key, e => e.ToArray());
	public static IReadOnlyDictionary<K, U[]> GroupInDict<T, U, K>(this IEnumerable<T> source, Func<T, K> funKey, Func<T, U> valFun) where K : notnull => source.GroupBy(funKey).ToDictionary(e => e.Key, e => e.SelectA(valFun));

	

	public static void EnsureEquivalent<T>(this IEnumerable<T> source, Func<T, bool> funA, Func<T, bool> funB, [CallerArgumentExpression(nameof(funA))] string? funAName = null, [CallerArgumentExpression(nameof(funB))] string? funBName = null)
	{
		var xs = source.ToArray();
		if (xs.Any(e => funA(e) ^ funB(e)))
		{
			var errs = xs.WhereA(e => funA(e) ^ funB(e));
			errs.Take(10).Dump();
			throw new ArgumentException($"{typeof(T).Name} EquivalenceFail (x{errs.Length}): {funAName} <=> {funBName}");
		}
	}

	public static T[] EnsureUniqueBy<T, K>(this IEnumerable<T> source, Func<T, K> keyFun, [CallerArgumentExpression(nameof(keyFun))]string? keyFunName = null) where K : notnull
	{
		var arrAll = source.ToArray();
		var arrUniqLng = arrAll.DistinctBy(keyFun).Count();
		if (arrUniqLng != arrAll.Length)
		{
			var errMsg = $"{typeof(T).Name} is not unique by {keyFunName}";
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

	public static T[] EnsureUniqueByOld<T, K>(this IEnumerable<T> source, Func<T, K> keyFun, string err) where K : notnull
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