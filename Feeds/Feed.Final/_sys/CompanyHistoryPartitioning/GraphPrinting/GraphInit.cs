using LINQPad;

namespace Feed.Final._sys.CompanyHistoryPartitioning.GraphPrinting;

static class GraphInit
{
	//const string ResourceFolder = "Feed.Final._sys.CompanyHistoryPartitioning.GraphPrinting";

	public static void Init()
	{
		//Util.HtmlHead.AddScript(ResourceLoader.Load($"{ResourceFolder}.leader-line.min.js"));

		Util.HtmlHead.AddScriptFromUri("https://cdn.jsdelivr.net/npm/mermaid@11/dist/mermaid.js");
		Util.HtmlHead.AddStyles(GraphStyles.MermaidCss);
		Util.HtmlHead.AddScript(GraphStyles.MermaidInit);
	}
}