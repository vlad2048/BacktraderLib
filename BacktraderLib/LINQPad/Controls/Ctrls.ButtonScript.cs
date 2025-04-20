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
				using var _ = web.UseCancelToken(cancelToken);
				await ScriptWatcher.Run(ScriptSource.QuerySection, web);
			}
		);
}