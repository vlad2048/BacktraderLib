using BacktraderLib.Utils;
using LINQPad;

namespace BacktraderLib._sys.JQuery;

static class JQueryInit
{
	const string ResourceFolder = "BacktraderLib.LINQPad.Controls._sys.JQuery";

	public static void Init()
	{
		Util.HtmlHead.AddScript(ResourceLoader.Load($"{ResourceFolder}.jquery.min.js"));
	}
}