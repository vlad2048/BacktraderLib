using System.Drawing;
using BaseUtils;
using Feed.SEC._sys.Rows;

namespace Feed.SEC._sys.UtilsUI;

static class QuarterBitmapMaker
{
	const int YW = 1;
	const int QW = 1;
	const int H = 8;
	static readonly Color colorBack = color(0x23457F);
	static readonly Brush brushY = brush(0x4B5FAF);
	static readonly Brush brushQ = brush(0x0f9131);

	public static Bitmap ToQuarterBmp(this IEnumerable<SubRow> xs)
	{
		var set = xs.Select(GetQuarter).ToHashSet();
		var width = Quarter.All.Count(e => e.Q is QNum.Q1) * (QW + YW) + Quarter.All.Count(e => e.Q is not QNum.Q1) * QW + YW;
		var height = H;
		var bmp = new Bitmap(width, height);
		using var gfx = Graphics.FromImage(bmp);
		gfx.Clear(colorBack);
		var x = 0;
		foreach (var q in Quarter.All)
		{
			if (q.Q is QNum.Q1)
			{
				gfx.FillRectangle(brushY, x, 0, YW, H);
				x += YW;
			}
			if (set.Contains(q))
			{
				gfx.FillRectangle(brushQ, x, 0, QW, H);
			}
			x += QW;
		}
		gfx.FillRectangle(brushY, 0, 0, width, 1);
		gfx.FillRectangle(brushY, 0, height - 1, width, 1);
		gfx.FillRectangle(brushY, width - YW, 0, YW, height);
		return bmp;
	}



	static Color color(int v) => Color.FromArgb(0xff, Color.FromArgb(v));
	static Brush brush(int v) => new SolidBrush(color(v));


	static Quarter GetQuarter(SubRow sub) => new(
		sub.Fy,
		sub.Fp switch
		{
			"Q1" => QNum.Q1,
			"Q2" => QNum.Q2,
			"Q3" => QNum.Q3,
			"FY" => QNum.Q4,
			_ => throw new ArgumentException("Impossible"),
		}
	);
}
