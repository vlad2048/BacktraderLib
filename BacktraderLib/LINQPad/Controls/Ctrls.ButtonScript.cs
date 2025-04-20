using LINQPad;
using ScrapeUtils;

namespace BacktraderLib;

public static partial class Ctrls
{
	static int runCount;

	public static Tag ButtonScript(Web web) =>
		ButtonCancellable(
			"Script",
			async cancelToken =>
			{
				web.Log.UpdateContent(Util.RawHtml($"<h2>Script run {runCount++}</h2>"));
				await ScriptWatcher.Run(ScriptSource.QuerySection, web, cancelToken);
			}
		);
}