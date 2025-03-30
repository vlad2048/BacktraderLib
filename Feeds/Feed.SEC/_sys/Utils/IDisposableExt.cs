namespace Feed.SEC._sys.Utils;

static class IDisposableExt
{
	public static void Dispose<K, V>(this IReadOnlyDictionary<K, V> map) where V : IDisposable
	{
		foreach (var val in map.Values)
			val.Dispose();
	}
}