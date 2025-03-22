namespace BacktraderLib;

public static class ColorLightener
{
	public static Color Lighten(this Color color, double amount = 0.7)
	{
		if (color is not ColorRGBA e) throw new ArgumentException("Can only lighten ColorRGBA types of colors");
		var hslSrc = e.ToHsl();
		var hslDst = hslSrc with { Light = (amount * hslSrc.Light).Clamp(0, 100) };
		return hslDst.ToRgb();
	}

	sealed record ColorHsl(byte Alpha, double Hue, double Saturation, double Light);


	static ColorHsl ToHsl(this ColorRGBA e)
	{
		var varR = e.R / 255.0;
		var varG = e.G / 255.0;
		var varB = e.B / 255.0;

		var varMin = Min(varR, varG, varB);
		var varMax = Max(varR, varG, varB);
		var delMax = varMax - varMin;

		double h;
		double s;
		var l = (varMax + varMin) / 2;

		if (Math.Abs(delMax - 0) < 0.00001)
		{
			h = 0;
			s = 0;
		}
		else
		{
			s = l switch
			{
				< 0.5 => delMax / (varMax + varMin),
				_ => delMax / (2.0 - varMax - varMin),
			};

			var delR = ((varMax - varR) / 6.0 + delMax / 2.0) / delMax;
			var delG = ((varMax - varG) / 6.0 + delMax / 2.0) / delMax;
			var delB = ((varMax - varB) / 6.0 + delMax / 2.0) / delMax;

			if (Math.Abs(varR - varMax) < 0.00001)
				h = delB - delG;
			else if (Math.Abs(varG - varMax) < 0.00001)
				h = 1.0 / 3.0 + delR - delB;
			else if (Math.Abs(varB - varMax) < 0.00001)
				h = 2.0 / 3.0 + delG - delR;
			else
				h = 0.0;

			if (h < 0.0)
				h += 1.0;
			if (h > 1.0)
				h -= 1.0;
		}

		return new ColorHsl(
			e.A,
			h * 360.0,
			s * 100.0,
			l * 100.0
		);
	}


	static Color ToRgb(this ColorHsl e)
	{
		double red, green, blue;

		var h = e.Hue / 360.0;
		var s = e.Saturation / 100.0;
		var l = e.Light / 100.0;

		if (Math.Abs(s - 0.0) < 0.00001)
		{
			red = l;
			green = l;
			blue = l;
		}
		else
		{
			var var2 = l switch
			{
				< 0.5 => l * (1.0 + s),
				_ => l + s - s * l,
			};

			var var1 = 2.0 * l - var2;

			red = hue2Rgb(var1, var2, h + 1.0 / 3.0);
			green = hue2Rgb(var1, var2, h);
			blue = hue2Rgb(var1, var2, h - 1.0 / 3.0);
		}

		var nRed = Convert.ToInt32(red * 255.0);
		var nGreen = Convert.ToInt32(green * 255.0);
		var nBlue = Convert.ToInt32(blue * 255.0);

		return MakeARGB(e.Alpha, nRed, nGreen, nBlue);
	}

	static Color MakeARGB(int a, int r, int g, int b) => new ColorRGBA
	{
		R = (byte)r,
		G = (byte)g,
		B = (byte)b,
		A = (byte)a,
	};



	static double hue2Rgb(
		double v1,
		double v2,
		double vH)
	{
		if (vH < 0.0)
			vH += 1.0;
		if (vH > 1.0)
			vH -= 1.0;
		if (6.0 * vH < 1.0)
			return v1 + (v2 - v1) * 6.0 * vH;
		if (2.0 * vH < 1.0)
			return v2;
		if (3.0 * vH < 2.0)
			return v1 + (v2 - v1) * (2.0 / 3.0 - vH) * 6.0;
		return v1;
	}

	static double Clamp(this double x, double min, double max) => Math.Clamp(x, min, max);
	static double Min(double a, double b, double c) => Math.Min(Math.Min(a, b), c);
	static double Max(double a, double b, double c) => Math.Max(Math.Max(a, b), c);
}