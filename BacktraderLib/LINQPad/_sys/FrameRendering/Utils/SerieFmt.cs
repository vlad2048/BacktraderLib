namespace BacktraderLib._sys.FrameRendering.Utils;

sealed record SerieFmt(string Fmt)
{
	static readonly SerieFmt Empty = new(string.Empty);


	public string Format(double v) => string.Format(Fmt, v);


	public static SerieFmt Make(double[] xs)
	{
		static string MkFmt(int n) => $"{{0:F{n}}}";

		static int GetDecimalDigits(double v)
		{
			var str = v.ToString(System.Globalization.CultureInfo.InvariantCulture);
			var idx = str.IndexOf('.');
			return idx switch
			{
				-1 => 0,
				_ => str.Length - idx - 1,
			};
		}

		if (xs.Length == 0) return Empty;

		var digits = Math.Min(RendererConsts.MaxDecimalDigits, xs.Max(GetDecimalDigits));
		var fmt = MkFmt(digits);

		return new SerieFmt(fmt);
	}
}