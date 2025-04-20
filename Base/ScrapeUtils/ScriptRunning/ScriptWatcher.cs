using ScrapeUtils._sys;

namespace ScrapeUtils;

public static class ScriptWatcher
{
	public static async Task Run(IScriptSource source, Web web, CancellationToken cancelToken)
	{
		try
		{
			web.CancelToken = cancelToken;
			var codeUnits = source.Get();
			await ScriptRunner.Run(codeUnits, web);
		}
		catch (Exception ex)
		{
			web.Log.AppendContent(ex);
		}
		finally
		{
			web.CancelToken = null;
		}
	}
}