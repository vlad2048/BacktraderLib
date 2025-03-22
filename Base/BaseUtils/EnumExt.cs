#pragma warning disable CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
namespace BaseUtils;

public static class EnumExt
{
	public static T[] WhereA<T>(this IEnumerable<T> source, Func<T, bool> fun) => source.Where(fun).ToArray();
	public static U[] SelectA<T, U>(this IEnumerable<T> source, Func<T, U> fun) => source.Select(fun).ToArray();
	public static U[] SelectA<T, U>(this IEnumerable<T> source, Func<T, int, U> fun) => source.Select(fun).ToArray();
	public static U[] SelectManyA<T, U>(this IEnumerable<T> source, Func<T, IEnumerable<U>> fun) => source.SelectMany(fun).ToArray();
	public static U[] SelectManyA<T, U>(this IEnumerable<T> source, Func<T, int, IEnumerable<U>> fun) => source.SelectMany(fun).ToArray();
	public static bool IsSame<T>(this T[] xs, T[] ys) => xs.Length == ys.Length && xs.Zip(ys).All(t => Equals(t.First, t.Second));
	public static IReadOnlyDictionary<K, V> ToOrderedDictionary<T, K, V>(this IEnumerable<T> source, Func<T, K> keyFun, Func<T, V> valFun) => new OrderedDictionary<K, V>(source.Select(e => new KeyValuePair<K, V>(keyFun(e), valFun(e))));
}