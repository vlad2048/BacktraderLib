using ScrapeUtils._sys;

namespace ScrapeUtils;

public static class ScriptWatcher
{
	public static async Task Run(IScriptSource source, Web web)
	{
		try
		{
			var codeUnits = source.Get();
			await ScriptRunner.Run(codeUnits, web);
		}
		catch (Exception ex)
		{
			web.Log.AppendContent(ex);
		}
	}
}