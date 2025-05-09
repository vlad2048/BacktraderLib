namespace LINQPadPlus._sys.Utils;

static class EnumExt
{
	public static U[] SelectA<T, U>(this IEnumerable<T> source, Func<T, U> fun) => source.Select(fun).ToArray();
}