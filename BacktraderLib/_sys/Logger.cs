using System.Text;
using LINQPad.FSharpExtensions;

namespace BacktraderLib._sys;

enum LogCategory
{
	Plot,
	PlotEvents,
}


static class Logger
{
	public static Action<string> Make(LogCategory cat) =>
		Consts.EnabledLogCategories.Contains(cat) switch
		{
			true => str => LogMessage(cat, str),
			false => _ => { }
		};

	static void LogMessage(LogCategory cat, string msg)
	{
		var thread = Thread.CurrentThread;
		var sb = new StringBuilder();
		sb.Append($"[{DateTime.Now:HH:mm:ss.ffffff}]");
		sb.Append($"[{thread.Name} - {thread.ManagedThreadId}]");
		sb.Append($"[{cat}]");
		sb.Append(" - ");
		sb.Append(msg);
		var str = sb.ToString();

		str.Dump();
	}
}