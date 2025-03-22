namespace BacktraderLib;

public static class DispExt
{
	public static T D<T>(this T v, Disp d) where T : class, IDisposable
	{
		d.Add(v);
		return v;
	}

	public static Dictionary<K, V> D<K, V>(this Dictionary<K, V> dict, Disp d) where K : notnull where V : IDisposable
	{
		Disposable.Create(dict, dict_ =>
		{
			foreach (var v in dict_.Values)
				v.Dispose();
		}).D(d);
		return dict;
	}

	public static List<T> D<T>(this List<T> list, Disp d) where T : IDisposable
	{
		Disposable.Create(list, list_ =>
		{
			foreach (var v in list_)
				v.Dispose();
		}).D(d);
		return list;
	}
}