using BaseUtils;
using LINQPad;

namespace Feed.SEC._sys.Utils;

static class EnumExt
{
	public static T[] ExceptA<T>(this IEnumerable<T> first, IEnumerable<T> second) => first.Except(second).ToArray();
	public static T[] ExceptA<T>(this IEnumerable<T> first, IEnumerable<T> second, IEqualityComparer<T> equalityComparer) => first.Except(second, equalityComparer).ToArray();

	public static HashSet<K> EnsureUnique<T, K>(this T[] source, Func<T, K> keyFun, string msg)
	{
		var uniqs = source.Select(keyFun).Distinct().ToArray();
		if (uniqs.Length != source.Length)
			throw new ArgumentException(msg);
		return uniqs.ToHashSet();
	}

	public static void EnsureRefExists<T, K>(this T[] source, Func<T, K> keyFun, HashSet<K> keys, string msg)
	{
		if (source.Any(e => !keys.Contains(keyFun(e))))
		{
			var missing = source.WhereA(e => !keys.Contains(keyFun(e)));
			missing.Take(10).Dump();
			throw new ArgumentException($"{msg} (x{missing.Length})");
		}
	}

	public static IEnumerable<T> EnsureTrue<T>(this IEnumerable<T> source, Func<T, bool> predicate, string msg) =>
		source
			.Select(e =>
			{
				if (!predicate(e)) throw new ArgumentException(msg);
				return e;
			});
}