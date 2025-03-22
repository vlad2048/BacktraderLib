using LINQPad;

namespace BacktraderLib._sys.Slider;

static class SliderInit
{
	const string ResourceFolder = "BacktraderLib.LINQPad.Controls._sys.Slider";

	public static void Init()
	{
		Util.HtmlHead.AddStyles(ResourceLoader.Load($"{ResourceFolder}.ion.rangeSlider.min.css"));
		Util.HtmlHead.AddScript(ResourceLoader.Load($"{ResourceFolder}.jquery.min.js"));
		Util.HtmlHead.AddScript(ResourceLoader.Load($"{ResourceFolder}.ion.rangeSlider.min.js"));
	}
}