namespace BaseUtils;

public sealed record GenericMatch<T, U>(T X, U? Y, int Score) where T : class where U : class;


public static class GenericMatcher
{
	public static GenericMatch<T, U>[] Match<T, U>(
		T[] xs,
		U[] ys,
		Func<T, string> xFun,
		Func<U, string> yFun,
		Func<string, string> norm
	) where T : class where U : class
	{
		xs.EnsureUnique(xFun, norm);
		ys.EnsureUnique(yFun, norm);

		var matcher = new Matcher<T, U>(xs, ys, xFun, yFun, norm);
		for (var i = 0; i < 2; i++)
		{
			var maxD = 3 + 3 * i;
			matcher.ForwardMatch(maxD);
			matcher.BackwardMatch(maxD);
		}

		var orderMap = xs.Index().ToDictionary(t => t.Item, t => t.Index);

		return matcher
			.GetMatches()
			.OrderBy(e => orderMap[e.X])
			.ToArray();
	}


	sealed class Matcher<T, U> where T : class where U : class
	{
		readonly List<T> xs;
		readonly List<U> ys;
		readonly Func<T, string> xFun;
		readonly Func<U, string> yFun;
		readonly Func<string, string> norm;
		readonly List<GenericMatch<T, U>> matches = new();

		public GenericMatch<T, U>[] GetMatches()
		{
			var list = matches.ToList();
			list.AddRange(xs.Select(x => new GenericMatch<T, U>(x, null, 0)));
			return [.. list];
		}

		public Matcher(IEnumerable<T> xs, IEnumerable<U> ys, Func<T, string> xFun, Func<U, string> yFun, Func<string, string> norm) => (this.xs, this.ys, this.xFun, this.yFun, this.norm) = (xs.ToList(), ys.ToList(), xFun, yFun, norm);

		public void ForwardMatch(int maxDistance)
		{
			foreach (var x in xs.ToArray())
			{
				if (ys.Count == 0) break;
				var bestY = ys
					.Where(y => Distance(xFun(x), yFun(y), norm) <= maxDistance)
					.MinBy(y => Distance(xFun(x), yFun(y), norm));
				if (bestY != null)
				{
					matches.Add(new GenericMatch<T, U>(x, bestY, Distance(xFun(x), yFun(bestY), norm)));
					ys.Remove(bestY);
					xs.Remove(x);
				}
			}
		}

		public void BackwardMatch(int maxDistance)
		{
			foreach (var y in ys.ToArray())
			{
				if (xs.Count == 0) break;
				var bestX = xs
					.Where(x => Distance(xFun(x), yFun(y), norm) <= maxDistance)
					.MinBy(x => Distance(xFun(x), yFun(y), norm));
				if (bestX != null)
				{
					matches.Add(new GenericMatch<T, U>(bestX, y, Distance(xFun(bestX), yFun(y), norm)));
					xs.Remove(bestX);
					ys.Remove(y);
				}
			}
		}
	}




	static int Distance(string a, string b, Func<string, string> norm) => WordDistance.Calculate(norm(a), norm(b));



	static void EnsureUnique<T>(this IEnumerable<T> source, Func<T, string> fun, Func<string, string> norm)
	{
		var arr = source.SelectA(fun);
		if (arr.Distinct().Count() != arr.Length) throw new ArgumentException("Non unique names given to the matcher");

		arr = arr.SelectA(norm);
		if (arr.Distinct().Count() != arr.Length) throw new ArgumentException("Non unique names given to the matcher (after normalization)");
	}
}