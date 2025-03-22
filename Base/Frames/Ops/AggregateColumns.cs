namespace Frames;

public static partial class FrameUtils
{
	public static Serie<N> AggregateColumns<N, K1>(this Frame<N, K1> frame, Func<double[], double> fun) =>
		Serie.Make(
			frame.Name,
			frame.Index,
			frame.Index
				.Index().Select(t => t.Index)
				.Select(idx => fun(
					frame.Select(col => col.Values[idx]).ToArray()
				))
				.ToArray()
		);
}