namespace BaseUtils;

public static class WordDistance
{
	public static int Calculate(string a, string b) =>
		(string.IsNullOrEmpty(a), string.IsNullOrEmpty(b)) switch
		{
			(true, true) => 0,
			(true, false) => b.Length,
			(false, true) => a.Length,
			(false, false) => DistanceNotEmpty(a, b),
		};

	static int DistanceNotEmpty(string a, string b)
	{
		var lngA = a.Length;
		var lngB = b.Length;
		var distances = new int[lngA + 1, lngB + 1];

		// ReSharper disable EmptyEmbeddedStatement
		for (int i = 0; i <= lngA; distances[i, 0] = i++) ;
		for (int j = 0; j <= lngB; distances[0, j] = j++) ;
		// ReSharper restore EmptyEmbeddedStatement

		for (int i = 1; i <= lngA; i++)
		for (int j = 1; j <= lngB; j++)
		{
			var cost = b[j - 1] == a[i - 1] ? 0 : 1;
			distances[i, j] = Math.Min(
				Math.Min(distances[i - 1, j] + 1, distances[i, j - 1] + 1),
				distances[i - 1, j - 1] + cost
			);
		}
		return distances[lngA, lngB];
	}
}