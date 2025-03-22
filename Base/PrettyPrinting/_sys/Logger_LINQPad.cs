using System.Drawing;
using System.Text;
using LINQPad;

namespace PrettyPrinting._sys;


static class Logger_LINQPad
{
	public static object Log(TxtArray arr)
	{
		Color? curBack = null;
		Color? curFore = null;

		void PropagateColors(Color? back, Color? fore)
		{
			curBack = back ?? curBack;
			curFore = fore ?? curFore;
		}

		var sb = new StringBuilder();

		sb.MkDiv(() =>
		{
			foreach (var x in arr.Array)
			{
				PropagateColors(x.Back, x.Fore);
				sb.MkSpan(x with { Back = curBack, Fore = curFore });
			}
		});

		var str = sb.ToString();
		return Util.RawHtml(str);
	}

	static void MkDiv(this StringBuilder sb, Action content)
	{
		sb.Append("<div style='font-family:Consolas; font-size:14px; white-space:pre'>");
		content();
		sb.Append("</div>");
	}

	static void MkSpan(this StringBuilder sb, Txt txt)
	{
		sb.Append("<span");
		if (txt.Back.HasValue || txt.Fore.HasValue)
		{
			sb.Append(" style='");
			if (txt.Back.HasValue)
				sb.Append($"background-color:{txt.Back.Value.ToHex()}; ");
			if (txt.Fore.HasValue)
				sb.Append($"color:{txt.Fore.Value.ToHex()}; ");
			sb.Append("'");
		}
		sb.Append(">");

		var str = txt.Text; //.Replace(Environment.NewLine, "<br>");
		sb.Append(str);

		sb.Append("</span>");
	}

	static string ToHex(this Color c) => $"#{c.R:X2}{c.G:X2}{c.B:X2}";
}
