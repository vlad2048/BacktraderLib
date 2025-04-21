using BaseUtils;

namespace Frames;

public interface IColumnFilter;
sealed record PredicateColumnFilter<K1, K2>(Func<Frame<K1, K2>, bool> Predicate) : IColumnFilter;

public static class ColumnFilter
{
	public static IColumnFilter Predicate<K1, K2>(Func<Frame<K1, K2>, bool> Predicate) => new PredicateColumnFilter<K1, K2>(Predicate);
}

public static partial class FrameUtils
{
	public static Frame<N, K1, K2> Filter<N, K1, K2>(this Frame<N, K1, K2> frame, IColumnFilter filter) =>
		Frame.Make(
			frame.Name,
			frame.Index,
			frame
				.Apply(filter)
				.SelectA(e => (
					e.Name,
					e.SelectA(f => (
						f.Name,
						f.Values
					))
				))
		);

	static IEnumerable<Frame<K1, K2>> Apply<K1, K2>(this IEnumerable<Frame<K1, K2>> source, IColumnFilter filter) =>
		filter switch
		{
			PredicateColumnFilter<K1, K2> { Predicate: var predicate } => source.Where(predicate),
			_ => throw new ArgumentException($"Unknown IColumnFilter: {filter.GetType().Name}"),
		};
}