using System.Drawing;

namespace PrettyPrinting._sys;

static class Logger_Console
{
	public static void Log(TxtArray xs)
	{
		var isNewlineLast = false;
		foreach (var x in xs.Array)
			Write(x, ref isNewlineLast);
		if (!isNewlineLast) Console.WriteLine();
		SetBack(DefaultBack);
		SetFore(DefaultFore);
	}

	static void Write(Txt txt, ref bool isNewlineLast)
	{
		if (txt.Back.HasValue) SetBack(txt.Back.Value);
		if (txt.Fore.HasValue) SetFore(txt.Fore.Value);
		if (txt.Text != string.Empty)
		{
			Console.Write(txt.Text);
			isNewlineLast = txt.Text.EndsWith(Environment.NewLine);
		}
	}

	static readonly Color DefaultBack = Color.FromArgb(0x282C34);
	static readonly Color DefaultFore = Color.FromArgb(0xDCDFE4);

	const char EscChar = (char)0x1B;
	static void SetBack(Color c) => Console.Write($"{EscChar}[48;2;{c.R};{c.G};{c.B}m");
	static void SetFore(Color c) => Console.Write($"{EscChar}[38;2;{c.R};{c.G};{c.B}m");
}