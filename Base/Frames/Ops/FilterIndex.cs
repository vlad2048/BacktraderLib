using BaseUtils;
using Frames._sys.Utils;

namespace Frames;

public interface IIndexFilter;
sealed record RemoveBeforeIndexFilter(DateTime TMin, int TBuf) : IIndexFilter;
sealed record AlignWithIndexFilter(DateTime[] Index) : IIndexFilter;
sealed record RemoveDatesIndexFilter(DateTime[] Dates) : IIndexFilter;
sealed record TruncateStartUntilNoNaNsIndexFilter : IIndexFilter;

public static class IndexFilter
{
	public static IIndexFilter RemoveBefore(DateTime TMin, int TBuf = 0) => new RemoveBeforeIndexFilter(TMin, TBuf);
	public static IIndexFilter AlignWith(DateTime[] Index) => new AlignWithIndexFilter(Index);
	public static IIndexFilter RemoveDates(DateTime[] Dates) => new RemoveDatesIndexFilter(Dates);
	public static readonly IIndexFilter TruncateStartUntilNoNaNs = new TruncateStartUntilNoNaNsIndexFilter();
}

public static partial class FrameUtils
{
	public static Serie<N> Filter<N>(this Serie<N> serie, IIndexFilter filter)
	{
		var pred = filter.ToPredicate(serie.Index);
		return Serie.Make(
			serie.Name,
			serie.Index.Apply(pred),
			serie.Values.Apply(pred)
		);
	}

	public static Frame<N, K1> Filter<N, K1>(this Frame<N, K1> frame, IIndexFilter filter)
	{
		var pred = filter.ToPredicate(frame);
		return Frame.Make(
			frame.Name,
			frame.Index.Apply(pred),
			frame.SelectA(e => (
				e.Name,
				e.Values.Apply(pred)
			))
		);
	}

	public static Frame<N, K1, K2> Filter<N, K1, K2>(this Frame<N, K1, K2> frame, IIndexFilter filter)
	{
		var pred = filter.ToPredicate(frame);
		return Frame.Make(
			frame.Name,
			frame.Index.Apply(pred),
			frame.SelectA(e => (
				e.Name,
				e.SelectA(f => (
					f.Name,
					f.Values.Apply(pred)
				))
			))
		);
	}



	static T[] Apply<T>(this T[] arr, Func<int, bool> pred) => arr.Index().Where(t => pred(t.Index)).SelectA(t => t.Item);


	static Func<int, bool> ToPredicate<N, K1>(this IIndexFilter filter, Frame<N, K1> frame)
	{
		switch (filter)
		{
			case RemoveBeforeIndexFilter:
			case AlignWithIndexFilter:
			case RemoveDatesIndexFilter:
				return filter.ToPredicate(frame.Index);

			case TruncateStartUntilNoNaNsIndexFilter:
			{
				var idx = frame.Index.Index().FirstOrDefault(t => frame.All(f => !f.Values[t.Index].IsNaN()), (-1, default)).Index;
				if (idx == -1) throw new ArgumentException("IndexFilter: there's NaNs until the end");
				return i => i >= idx;
			}

			default:
				throw new ArgumentException("Unknown IndexFilter");
		}
	}


	static Func<int, bool> ToPredicate<N, K1, K2>(this IIndexFilter filter, Frame<N, K1, K2> frame)
	{
		switch (filter)
		{
			case RemoveBeforeIndexFilter:
			case AlignWithIndexFilter:
			case RemoveDatesIndexFilter:
				return filter.ToPredicate(frame.Index);

			case TruncateStartUntilNoNaNsIndexFilter:
			{
				var idx = frame.Index.Index().FirstOrDefault(t => frame.All(f => f.All(g => !g.Values[t.Index].IsNaN())), (-1, default)).Index;
				if (idx == -1) throw new ArgumentException("IndexFilter: there's NaNs until the end");
				return i => i >= idx;
			}

			default:
				throw new ArgumentException("Unknown IndexFilter");
		}
	}



	static Func<int, bool> ToPredicate(this IIndexFilter filter, DateTime[] arr)
	{
		switch (filter)
		{
			case RemoveBeforeIndexFilter e:
			{
				var idx = arr.Index().FirstOrDefault(t => t.Item >= e.TMin, (-1, default)).Index;
				if (idx == -1) throw new ArgumentException("IndexFilter: returns no values");
				idx -= e.TBuf;
				if (idx < 0) throw new ArgumentException("IndexFilter: not enough values for the buffer");
				return i => i >= idx;
			}

			case AlignWithIndexFilter e:
			{
				var (tMin, tMax) = (e.Index[0], e.Index[^1]);
				return i => arr[i] >= tMin && arr[i] <= tMax;
			}

			case RemoveDatesIndexFilter e:
			{
				var set = e.Dates.ToHashSet();
				return i => !set.Contains(arr[i]);
			}

			case TruncateStartUntilNoNaNsIndexFilter:
				throw new ArgumentException("Not handled here");

			default:
				throw new ArgumentException("Unknown IndexFilter");
		}
	}
}