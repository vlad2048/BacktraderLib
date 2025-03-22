using BaseUtils;

namespace Frames;

public static partial class FrameUtils
{
	public static Serie<N2> WithName<N1, N2>(this Serie<N1> serie, N2 name)
		=> Serie.Make(
			name,
			serie.Index,
			serie.Values
		);

	public static Frame<N2, K1> WithName<N1, N2, K1>(this Frame<N1, K1> frame, N2 name)
		=> Frame.Make(
			name,
			frame.Index,
			frame.SelectA(e => (
				e.Name,
				e.Values
			))
		);
	public static Frame<N2, K1, K2> WithName<N1, N2, K1, K2>(this Frame<N1, K1, K2> frame, N2 name)
		=> Frame.Make(
			name,
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