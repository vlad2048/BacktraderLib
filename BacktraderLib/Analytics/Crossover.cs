using BacktraderLib._sys;
using BaseUtils;
using Frames;

namespace BacktraderLib;

public static class CrossoverLogic
{
	public static (Frame<string, K1>, Frame<string, K1>) Crossover<K1>(this Frame<string, K1> frameA, Frame<string, K1> frameB)
	{
		frameA.CheckIfCompatibleWith(frameB);
		var signals = frameA.Zip(frameB).SelectA(t => new
		{
			t.First.Name,
			Arrays = Calc(t.First.Values, t.Second.Values),
		});
		return (
			Frame.Make(
				"Entries",
				frameA.Index,
				signals.SelectA(e => (e.Name, e.Arrays.Item1))
			),
			Frame.Make(
				"Exits",
				frameA.Index,
				signals.SelectA(e => (e.Name, e.Arrays.Item2))
			)
		);
	}

	public static (double[], double[]) Calc(double[] xs, double[] ys) => (
		CrossAbove(xs, ys),
		CrossBelow(xs, ys)
	);


	static double[] CrossBelow(this double[] xs, double[] ys) => ys.CrossAbove(xs);

	static double[] CrossAbove(this double[] xs, double[] ys)
	{
		var n = xs.Length;
		if (ys.Length != n) throw new ArgumentException("Different sizes");
		var rs = new double[n];
		var wasBelow = false;
		var crossedAgo = -1;

		for (var i = 0; i < n; i++)
		{
			var (x, y) = (xs[i], ys[i]);
			if (x.IsNaN() || y.IsNaN())
			{
				crossedAgo = -1;
				wasBelow = false;
				rs[i] = 0;
			}
			else if (x > y)
			{
				if (wasBelow)
				{
					crossedAgo++;
					rs[i] = crossedAgo == 0 ? 1 : 0;
				}
				else
				{
					rs[i] = 0;
				}
			}
			// ReSharper disable once CompareOfFloatsByEqualityOperator
			else if (x == y)
			{
				crossedAgo = -1;
				rs[i] = 0;
			}
			else
			{
				crossedAgo = -1;
				wasBelow = true;
				rs[i] = 0;
			}
		}

		return rs;
	}
}