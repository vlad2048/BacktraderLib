namespace BacktraderLib;

public static class DoubleExt
{
	// ReSharper disable once CompareOfFloatsByEqualityOperator
	public static bool IsTrue(this double e) => e == 1.0;
}