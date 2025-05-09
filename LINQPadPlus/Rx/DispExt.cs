namespace LINQPadPlus.Rx;

public static class DispExt
{
	public static T D<T>(this T v, Disp d) where T : class, IDisposable
	{
		d.Add(v);
		return v;
	}
}