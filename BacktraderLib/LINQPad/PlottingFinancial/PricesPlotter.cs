using BaseUtils;
using Frames;

namespace BacktraderLib;

public static class PricesPlotter
{
	static readonly Layout defaultLayout = new()
	{
		Width = 600,
		Height = 250,
		Margin = new Marg(30, 30, 30, 30, 6, true),
		Template = Themes.Dark,
		Legend = new()
		{
			Orientation = Orientation.H,
			Xanchor = XAnchor.Right,
			X = 1.0,
			Yanchor = YAnchor.Bottom,
			Yref = AnchorRef.Paper,
			Y = 1.02,
		},
	};
	static readonly Config defaultConfig = new();


	public static Plot Plot(this Frame<string, string, Bar> prices) =>
		BacktraderLib.Plot.Make(
			prices.SelectA(e => e[Bar.Close].ToTrace(e.Name)),
			defaultLayout,
			defaultConfig
		);
}