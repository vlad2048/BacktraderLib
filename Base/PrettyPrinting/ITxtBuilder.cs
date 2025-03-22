using System.Drawing;

namespace PrettyPrinting;

public interface ITxtBuilder
{
	void Write(string text, Color? back = null, Color? fore = null);
	void WriteLine();
}

public static class ITxtBuilderExts
{
	public static void SetBack(this ITxtBuilder b, Color back) => b.Write(string.Empty, back);
	public static void SetFore(this ITxtBuilder b, Color fore) => b.Write(string.Empty, null, fore);

	public static void WriteLine(this ITxtBuilder b, string text, Color? back = null, Color? fore = null)
	{
		b.Write(text, back, fore);
		b.WriteLine();
	}
}