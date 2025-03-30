using System.Diagnostics;
using System.Drawing;
using System.Text;
using LINQPad;

namespace Feed.SEC._sys.Utils;

static class Logger
{
	public static Action<string> Make(LogCategory cat) =>
		Consts.EnabledLogCategories.Contains(cat) switch
		{
			true => str => LogMessage(cat, str),
			false => _ => { },
		};

	static void LogMessage(LogCategory cat, string msg)
	{
		if (msg is "")
		{
			"".Dump();
			return;
		}

		var sb = new StringBuilder();
		sb.Append($"[{DateTime.Now:HH:mm:ss.ffffff}]");
		sb.Append($"[{cat}]");
		sb.Append(" - ");
		sb.Append(msg);
		sb.ToString().Dump();
	}
}



static class LoggerExt
{
	public static void Loop<T>(
		this T[] xs,
		Action<string> Log,
		int stepNum,
		string stepName,
		Func<T, string> fmt,
		Action<T> action
	)
	{
		Log.LogStep(stepNum, stepName, xs.Length);

		for (var i = 0; i < xs.Length; i++)
		{
			var x = xs[i];
			Log.Title($"[{i + 1}/{xs.Length}]    {fmt(x)}");

			var sw = Stopwatch.StartNew();
			action(x);
			Log($"time:{(int)sw.Elapsed.TotalSeconds}s");

			if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Q)
			{
				Log("User interrupted");
				break;
			}
		}
	}

	public static void Title(this Action<string> Log, string s)
	{
		Log("");
		Log(s);
		Log(new string('=', s.Length));
	}

	public static string FmtArchFile(this string archFile) =>
		$"{Path.GetFileName(archFile)}    {new FileInfo(archFile).Length / 1000:n0} kb";


	static void LogStep(this Action<string> Log, int num, string name, int todo)
	{
		var n = name.Length;
		var line = new string('═', n);
		var s = $"{num}/{Consts.StepCount}";
		Log("");
		Log($"╔══════════╦═{line}═╗");
		Log($"║ Step {s} ║ {name} ║    todo:{todo}");
		Log($"╚══════════╩═{line}═╝");
	}
}