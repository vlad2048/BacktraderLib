using System.Text.Json.Serialization;

namespace _sys.Logging.Structs;

readonly record struct Pt(int X, int Y);

readonly record struct R(int X, int Y, int Width, int Height)
{
	public static readonly R Empty = new(0, 0, 0, 0);

	[JsonIgnore]
	public Sz Size => new(Width, Height);

	public R(Pt pos, Sz size) : this(pos.X, pos.Y, size.Width, size.Height)
	{
	}
}

readonly record struct Sz(int Width, int Height);


static class SzExt
{
	public static Sz MakeBigger(this Sz sz, Sz delta) => new(sz.Width + delta.Width, sz.Height + delta.Height);
	public static Sz MakeSmaller(this Sz sz, Sz delta) => new(sz.Width - delta.Width, sz.Height - delta.Height);
}


static class RExt
{
	public static R Union(this IEnumerable<R> listE)
	{
		var list = listE.Where(e => e != R.Empty).ToArray();
		if (list.Length == 0) return R.Empty;
		var minX = list.Min(e => e.X);
		var minY = list.Min(e => e.Y);
		var maxX = list.Max(e => e.X + e.Width);
		var maxY = list.Max(e => e.Y + e.Height);
		return new R(minX, minY, maxX - minX, maxY - minY);
	}

	/*
	public static R Intersection(this R a, R b)
	{
		var x = Math.Max(a.X, b.X);
		var num1 = Math.Min(a.X + a.Width, b.X + b.Width);
		var y = Math.Max(a.Y, b.Y);
		var num2 = Math.Min(a.Y + a.Height, b.Y + b.Height);
		return num1 >= x && num2 >= y ? new R(x, y, num1 - x, num2 - y) : R.Empty;
	}

	public static R Intersection(this IEnumerable<R> source)
	{
		var arr = source.ToArray();
		if (arr.Length == 0) return R.Empty;
		var curR = arr[0];
		for (var i = 1; i < arr.Length; i++)
			curR = curR.Intersection(arr[i]);
		return curR;
	}

	public static R CapToMin(this R r, int minWidth, int minHeight) => r with { Width = Math.Max(r.Width, minWidth), Height = Math.Max(r.Height, minHeight) };
	public static R WithZeroPos(this R r) => new(Pt.Empty, r.Size);
	public static R SzDec(this R r) => r with { Width = Math.Max(0, r.Width - 1), Height = Math.Max(0, r.Height - 1) };
	public static R WithSize(this R r, Sz sz) => new(r.Pos, sz);

	public static R Enlarge(this R r, int v)
	{
		if (v < 0) throw new ArgumentException();
		return r.EnlargeShrinkSigned(v);
	}

	public static R Shrink(this R r, int v)
	{
		if (v < 0) throw new ArgumentException();
		return r.EnlargeShrinkSigned(-v);
	}

	private static R EnlargeShrinkSigned(this R r, int v)
	{
		if (v >= 0)
		{
			return new R(r.X - v, r.Y - v, r.Width + v * 2, r.Height + v * 2);
		}
		else
		{
			v = -v;
			var left = r.X + v;
			var top = r.Y + v;
			var right = r.Right - v;
			var bottom = r.Bottom - v;
			if (left > right)
				left = right = (left + right) / 2;
			if (top > bottom)
				top = bottom = (top + bottom) / 2;
			return new R(left, top, right - left, bottom - top);
		}
	}
	*/
}
