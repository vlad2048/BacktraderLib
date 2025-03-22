using BacktraderLib._sys;
using BacktraderLib.Shapers;
using Frames;

namespace BacktraderLib;

public static class SymbolPlotter
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


	public static Plot<ScatterTrace> ToSymbolPlot<I>(
		this IRoVar<I> Δinp,
		IRoVar<string> Δsym,
		Frame<string, string, Bar> prices,
		Func<I, string, ScatterTrace[]> extraTraces,
		Layout? layout = null,
		Config? config = null,
		Action<PlotOpts>? optFun = null
	) where I : IWithSimResult
	{
		var timeOfs = Δinp.V.SimResult.TimeOfs;
		var Δdata = Var.Expr(() => new Data<I>(Δinp.V, Δsym.V));
		var plot = Plot.Make<ScatterTrace>(
			[],
			ObjectMerger.MergeOpt(defaultLayout, layout),
			ObjectMerger.MergeOpt(defaultConfig, config),
			opt =>
			{
				opt.FixXAxis = (prices.Index[timeOfs], prices.Index[^1]);
				optFun?.Invoke(opt);
			}
		);

		ScatterTrace[] MakeTraceUpdates(Data<I> data) => [
			prices[data.Sym, Bar.Close].ToTrace(),
			..extraTraces(data.Inp, data.Sym),
			data.Inp.SimResult.Syms[data.Sym].Orders.ToBuyMarkersTrace(prices.Index),
			data.Inp.SimResult.Syms[data.Sym].Orders.ToSellMarkersTrace(prices.Index),
		];

		Δdata.Subscribe(data => plot.Update(
			MakeTraceUpdates(data).Index().ToArray(),
			new Layout
			{
				Shapes = data.Inp.SimResult.Syms[data.Sym].Trades.ToTradeShapes(prices.Index),
			}
		));
		return plot;
	}


	sealed record Data<I>(
		I Inp,
		string Sym
	);
}