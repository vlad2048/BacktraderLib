using BaseUtils;

namespace Frames;

public static partial class FrameUtils
{
	public static Serie<N> Clone<N>(this Serie<N> serie)
		=> Serie.Make(
			serie.Name,
			serie.Index,
			serie.Values
		);

	public static Frame<N, K1> Clone<N, K1>(this Frame<N, K1> frame)
		=> Frame.Make(
			frame.Name,
			frame.Index,
			frame.SelectA(e => (
				e.Name,
				e.Values
			))
		);

	public static Frame<N, K1, K2> Clone<N, K1, K2>(this Frame<N, K1, K2> frame)
		=> Frame.Make(
			frame.Name,
			frame.Index,
			frame.SelectA(e => (
				e.Name,
				e.SelectA(f => (
					f.Name,
					f.Values
				))
			))
		);
}