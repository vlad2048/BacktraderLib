namespace BacktraderLib._sys.FrameRendering.Utils;

static class FrameRenderingUtils
{
	public static string[] MakeRootStyle(int? width) => width switch
	{
		not null =>
		[
			$"width: {width.Value}px",
			"display: inline-block",
		],
		null => [],
	};
}