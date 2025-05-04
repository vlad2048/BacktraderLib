using LINQPad;

namespace BacktraderLib._sys;

static class TableInit
{
	const string ResourceFolder = "BacktraderLib.LINQPad.Controls.Table._sys";

	public static void Init()
	{
		Util.HtmlHead.AddStyles(ResourceLoader.Load($"{ResourceFolder}.tabulator_site_dark.min.css"));
		Util.HtmlHead.AddScript(ResourceLoader.Load($"{ResourceFolder}.tabulator.min.js"));
		Util.HtmlHead.AddScript(
			"""
			function tabulator_formatMoney(v) {
			    let sym = '';
			    if (v >= 1_000_000_000) {
			        v /= 1_000_000_000;
			        sym = ' B';
			    } else if (v >= 1_000_000) {
			        v /= 1_000_000;
			        sym = ' M';
			    } else if (v >= 1_000) {
			        v /= 1_000;
			        sym = ' K';
			    }
			    const vStr = v.toLocaleString('en-US', {
			        minimumFractionDigits: 2,
			        maximumFractionDigits: 2,
			    });
			    return `$${vStr}${sym}`;
			}
			"""
		);
	}
}
