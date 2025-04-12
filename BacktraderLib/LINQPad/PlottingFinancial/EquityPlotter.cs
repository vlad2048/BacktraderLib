using BacktraderLib._sys;
using Frames;
using RxLib;

namespace BacktraderLib;

public static class EquityPlotter
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


	public static Plot ToEquityPlot<I>(
		this IRoVar<I> Δinp,

		Frame<string, string, Bar> prices,
		Frame<string, string, Bar>? pricesIndex = null,
		Layout? layout = null,
		Config? config = null,
		Action<PlotOpts>? optFun = null
	) where I : IWithSimResult
	{
		var timeOfs = Δinp.V.SimResult.TimeOfs;
		var cashStart = Δinp.V.SimResult.CashStart;
		var index = Δinp.V.SimResult.Equity.Index;

		var buyholdPrices = BuyAndHoldEquityCalc.BuyAndHold(prices, timeOfs, cashStart).WithName("Buy & Hold");
		var buyholdIndex = pricesIndex switch
		{
			not null => BuyAndHoldEquityCalc.BuyAndHold(pricesIndex, timeOfs, cashStart).WithName("Buy & Hold (Index)"),
			null => null,
		};

		var plot = Plot.Make(
			[
				Δinp.V.SimResult.Equity.ToTrace(),
				buyholdPrices.ToTrace(),
				..If(buyholdIndex != null, () => buyholdIndex!.ToTrace()),
			],
			ObjectMerger.MergeOpt(defaultLayout, layout),
			ObjectMerger.MergeOpt(defaultConfig, config),
			opt =>
			{
				opt.FixXAxis = (index[timeOfs], index[^1]);
				optFun?.Invoke(opt);
			}
		);

		Δinp.Subscribe(inp => plot.Update([
			(0, inp.SimResult.Equity.ToTrace()),
		]));

		return plot;
	}

	static T[] If<T>(bool condition, Func<T> fun) => condition switch
	{
		false => [],
		true => [fun()],
	};
}