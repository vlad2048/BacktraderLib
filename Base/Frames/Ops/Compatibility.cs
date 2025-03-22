using BaseUtils;

namespace Frames;

public static partial class FrameUtils
{
	public static void CheckIfCompatibleWith<N, K1>(this Frame<N, K1> frameA, Frame<N, K1> frameB)
	{
		if (!frameA.Index.IsSame(frameB.Index)) throw new ArgumentException("Incompatible frame: different Index");
		if (!frameA.SelectA(e => e.Name).IsSame(frameB.SelectA(e => e.Name))) throw new ArgumentException("Incompatible frame: different Columns");
	}
}