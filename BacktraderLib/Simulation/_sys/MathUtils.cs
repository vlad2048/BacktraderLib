namespace BacktraderLib._sys;

static class MathUtils
{
	const double Epsilon = 1e-12;

	public static bool IsNegative(this double value) => value <= -Epsilon;

	public static double SnapToZeroIfClose(this double value) =>
		(value > -Epsilon && value < Epsilon && value != 0) switch
		{
			true => 0,
			false => value,
		};
}