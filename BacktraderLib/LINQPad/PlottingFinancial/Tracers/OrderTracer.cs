namespace BacktraderLib;

public static class OrderTracer
{
	public static ScatterTrace ToBuyMarkersTrace(this OrderUpdate[] orders, DateTime[] index) => orders.ToOrderMarkersTrace(
		index,
		OrderDir.Buy,
		Symbol.TriangleUp,
		Colors.Green,
		"Buy"
	);

	public static ScatterTrace ToSellMarkersTrace(this OrderUpdate[] orders, DateTime[] index) => orders.ToOrderMarkersTrace(
		index,
		OrderDir.Sell,
		Symbol.TriangleDown,
		Colors.Red,
		"Sell"
	);

	static ScatterTrace ToOrderMarkersTrace(this OrderUpdate[] orders, DateTime[] index, OrderDir dir, Symbol symbol, Color color, string name)
	{
		var xs = orders
			.FilterFills()
			.Where(e => e.Order.Dir == dir).ToArray();
		return new ScatterTrace
		{
			X = xs.Select(e => index[e.Date]).ToArray(),
			Y = xs.Select(e => e.ExecPrice).ToArray(),
			Name = name,
			Mode = ScatterMode.Markers,
			Marker = new Marker
			{
				Symbol = symbol,
				Size = 8,
				Color = color,
				Line = new MarkerLine
				{
					Width = 1,
					Color = color.Lighten(),
				},
			},
		};
	}
}