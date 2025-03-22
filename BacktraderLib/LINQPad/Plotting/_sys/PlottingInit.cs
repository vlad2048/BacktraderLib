using LINQPad;

namespace BacktraderLib._sys;

static class PlottingInit
{
	public static DumpContainer ErrorDC = null!;

	public static void Init()
	{
		Util.HtmlHead.AddScriptFromUri(Consts.PlotlyScriptUrl);
		JS.Run(
			"""
			Plotly.setPlotConfig({
			    plotlyServerURL: 'https://chart-studio.plotly.com',
			    displaylogo: false,
			    modeBarButtonsToRemove: ['toImage', 'select2d', 'lasso2d', 'zoomIn2d', 'zoomOut2d' ],
			    logging: 2,
			    notifyOnLogging: 2,
			}); 
			"""
		);
		ErrorDC = new DumpContainer().Dump();
	}
}