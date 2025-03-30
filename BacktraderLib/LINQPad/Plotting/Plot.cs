using BacktraderLib._sys;
using BacktraderLib._sys.Events_;

namespace BacktraderLib;



public sealed class PlotOpts
{
	public (FlexValue, FlexValue)? FixXAxis { get; set; }

	PlotOpts()
	{
	}

	internal static PlotOpts Build(Action<PlotOpts>? optFun)
	{
		var opts = new PlotOpts();
		optFun?.Invoke(opts);
		return opts;
	}
}




public static class Plot
{
	public static Plot<T> Make<T>(
		T[] traces,
		Layout? layout = null,
		Config? config = null,
		Action<PlotOpts>? optFun = null
	) where T : ITrace, new() => new(
		traces,
		layout ?? new Layout(),
		config ?? new Config(),
		PlotOpts.Build(optFun)
	);
}





// Inherit from Tag
// ----------------
public sealed class Plot<T> : Tag where T : ITrace, new()
{
	readonly Action<string> Log = Logger.Make(LogCategory.Plot);

	readonly TraceLayoutKeeper<T> traceLayout;


	public IObservable<ClickArgs> WhenClick { get; }


	internal Plot(
		T[] traces,
		Layout layout,
		Config config,
		PlotOpts opts
	) : base("div")
	{
		Id = IdGen.Make();
		var isRendered = Var.Make(false);
		traceLayout = new TraceLayoutKeeper<T>(
			traces,
			layout,
			isRendered,
			(updateTraces, updateLayout) => Plotly.Update(
				Id,
				updateTraces,
				updateLayout.ApplyFixXAxis(opts.FixXAxis)
			)
		);

		WhenClick = EventDefs.Click.Make<ClickArgs>(Id, isRendered);



		OnRender = () =>
		{
			Log("OnRender");

			var (traces_, layout_) = traceLayout.OnDump();
			var js = Plotly.NewPlot(
				Id,
				traces_,
				layout_.ApplyFixXAxis(opts.FixXAxis),
				config
			);
			JS.Run(js);
			isRendered.V = true;
		};
	}



	public void Update((int, T)[] updatesTrace) => Update(updatesTrace, new Layout());
	public void Update(Layout updateLayout) => Update([], updateLayout);

	public void Update((int, T)[] updatesTrace, Layout updateLayout)
	{
		Log("UpdateTracesLayout");
		traceLayout.OnUpdate(updatesTrace, updateLayout);
	}

	public void WriteDebugFile() => File.WriteAllText(Consts.PlotlyDebugFile, Plotly.GetJSCode(Id ?? throw new ArgumentException("Impossible")));
}





file static class PlotUtils
{
	public static Layout ApplyFixXAxis(this Layout layout, (FlexValue, FlexValue)? fixXAxis) =>
		fixXAxis switch
		{
			null => layout,
			not null => layout with
			{
				Xaxis = new Axis
				{
					Range = FlexArray.FromValues([fixXAxis.Value.Item1, fixXAxis.Value.Item2]),
				},
			},
		};
}