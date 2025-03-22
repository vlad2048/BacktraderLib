namespace BacktraderReverser.Structs;

public sealed record Prices(
	double[] Open,
	double[] Close,
	double[]? High,
	double[]? Low
)
{
	public int Length => Open.Length;

	public static Prices Gen(
		int length,
		bool genHighLow,
		int? seed
	)
	{
		var rnd = seed switch
		{
			not null => new Random(seed.Value),
			null => new Random(),
		};

		var (open, close) = (new double[length], new double[length]);
		var (high, low) = genHighLow switch
		{
			true => (new double[length], new double[length]),
			false => (null, null),
		};
		for (var i = 0; i < length; i++)
		{
			var mid = rnd.Next(20, 80);
			var openV = mid + rnd.Next(-10, 11);
			var closeV = mid + rnd.Next(-10, 11);
			open[i] = openV;
			close[i] = closeV;
			var (min, max) = (Math.Min(openV, closeV), Math.Max(openV, closeV));
			if (genHighLow)
			{
				high![i] = rnd.Next(min - 5, min + 1);
				low![i] = rnd.Next(max, max + 6);
			}
		}

		return new Prices(open, close, high, low);
	}
}