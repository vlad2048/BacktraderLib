using System.Text;
using LINQPad;

namespace Feed.Trading212._sys.Utils;


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
		var catStr = $"{cat}".PadRight(12);
		var sb = new StringBuilder();
		sb.Append($"[{DateTime.Now:HH:mm:ss.ffffff}]");
		//sb.Append($"[{Thread.CurrentThread.Name} - {Thread.CurrentThread.ManagedThreadId}]");
		sb.Append($"[{catStr}]");
		sb.Append(" ");
		sb.Append(msg);
		var str = sb.ToString();

		str.Dump();
	}
}



static class LoggerExt
{
	public static void BigTitle(this Action<string> Log, string s)
	{
		s = $"* {s} *";
		var pad = new string('*', s.Length);
		Log("");
		Log(pad);
		Log(s);
		Log(pad);
	}


	public static void Title(this Action<string> Log, string s)
	{
		Log("");
		Log(s);
		Log(new string('=', s.Length));
	}
}