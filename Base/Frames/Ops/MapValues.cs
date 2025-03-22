using BaseUtils;

namespace Frames;

public static partial class FrameUtils
{
	public static Serie<N> MapValues<N>(this Serie<N> serie, Func<double[], double[]> fun)
		=> Serie.Make(
			serie.Name,
			serie.Index,
			fun(serie.Values)
		);

	public static Frame<N, K1> MapValues<N, K1>(this Frame<N, K1> frame, Func<double[], double[]> fun)
		=> Frame.Make(
			frame.Name,
			frame.Index,
			frame.SelectA(e => (
				e.Name,
				fun(e.Values)
			))
		);

	public static Frame<N, K1, K2> MapValues<N, K1, K2>(this Frame<N, K1, K2> frame, Func<double[], double[]> fun)
		=> Frame.Make(
			frame.Name,
			frame.Index,
			frame.SelectA(e => (
				e.Name,
				e.SelectA(f => (
					f.Name,
					fun(f.Values)
				))
			))
		);
}