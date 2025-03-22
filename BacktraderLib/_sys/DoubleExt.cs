namespace BacktraderLib._sys;

static class DoubleExt
{
	public static bool IsNaN(this double e) => double.IsNaN(e);
	public static bool IsNotNaN(this double e) => !double.IsNaN(e);
	public static double EnsureNotNaN(this double e)
	{
		if (double.IsNaN(e)) throw new ArgumentException("This number shouldn't be a NaN");
		return e;
	}
}