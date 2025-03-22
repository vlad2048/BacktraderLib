namespace BacktraderLib.Shapers;

public static class TradeShaper
{
	public static Shape[] ToTradeShapes(this Trade[] trades, DateTime[] index) =>
		trades
			.Select(e => new Shape()
			{
				Type = ShapeType.Rect,
				Xref = "x",
				Yref = "y",
				X0 = index[e.EntrDate],
				Y0 = e.EntrPrice,
				X1 = index[e.ExitDate],
				Y1 = e.ExitPrice,
				Fillcolor = e.Pnl switch
				{
					>= 0 => Color.Hex(0x008000),
					_ => Color.Hex(0xFF0000),
				},
				Opacity = 0.2,
				Layer = Layer.Below,
				Line = new ShapeLine()
				{
					Width = 0,
				},
			})
			.ToArray();
}