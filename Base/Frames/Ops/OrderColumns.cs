using BaseUtils;

namespace Frames;

public static partial class FrameUtils
{
	public static Frame<N, K1> OrderColumnsBy<N, K1, TKey>(this Frame<N, K1> frame, Func<Serie<K1>, TKey> keySelector)
		=> Frame.Make(
			frame.Name,
			frame.Index,
			frame
				.OrderBy(keySelector)
				.SelectA(e => (
					e.Name,
					e.Values
				))
		);
	public static Frame<N, K1> OrderColumnsByDescending<N, K1, TKey>(this Frame<N, K1> frame, Func<Serie<K1>, TKey> keySelector)
		=> Frame.Make(
			frame.Name,
			frame.Index,
			frame
				.OrderByDescending(keySelector)
				.SelectA(e => (
					e.Name,
					e.Values
				))
		);

	public static Frame<N, K1, K2> OrderColumnsBy<N, K1, K2, TKey>(this Frame<N, K1, K2> frame, Func<Frame<K1, K2>, TKey> keySelector)
		=> Frame.Make(
			frame.Name,
			frame.Index,
			frame
				.OrderBy(keySelector)
				.SelectA(e => (
					e.Name,
					e.SelectA(f => (
						f.Name,
						f.Values
					))
			))
		);
	public static Frame<N, K1, K2> OrderColumnsByDescending<N, K1, K2, TKey>(this Frame<N, K1, K2> frame, Func<Frame<K1, K2>, TKey> keySelector)
		=> Frame.Make(
			frame.Name,
			frame.Index,
			frame
				.OrderByDescending(keySelector)
				.SelectA(e => (
					e.Name,
					e.SelectA(f => (
						f.Name,
						f.Values
					))
				))
		);
}