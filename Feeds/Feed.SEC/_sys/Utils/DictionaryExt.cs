namespace Feed.SEC._sys.Utils;

static class DictionaryExt
{
	public static V GetOrAdd<K, V>(this Dictionary<K, V> map, K key, Func<V> create) where K : notnull =>
		map.TryGetValue(key, out var val) switch
		{
			true => val,
			false => map[key] = create(),
		};
}