using System.Diagnostics;
using System.Reflection;
using System.Text;
using BaseUtils;
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
		Action<T, int, int> action
	)
	{
		for (var i = 0; i < xs.Length; i++)
		{
			var x = xs[i];

			var sw = Stopwatch.StartNew();
			action(x, i + 1, xs.Length);
			Log($"time:{(int)sw.Elapsed.TotalSeconds}s");

			if (!IsRunningInLINQPad && Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Q)
			{
				Log("User interrupted");
				break;
			}
		}
	}

	static bool IsRunningInLINQPad => Assembly.GetEntryAssembly()?.GetName().Name == "LINQPad.Query";

	public static void Step(this Action<string> Log, Step step)
	{
		var all = Enum.GetValues<Step>().WhereA(e => e != SEC.Step.All);
		var idx = all.IdxOf(step);
		var cnt = all.Length;
		var name = $"{step}";
		var n = name.Length;
		var line = new string('═', n);
		var s = $"{idx + 1}/{cnt}";
		Log("");
		Log($"╔══════════╦═{line}═╗");
		Log($"║ Step {s} ║ {name} ║");
		Log($"╚══════════╩═{line}═╝");
	}

	public static void Title(this Action<string> Log, string s)
	{
		Log("");
		Log(s);
		Log(new string('=', s.Length));
	}

	public static string FmtArchFile(this string archFile) =>
		$"{Path.GetFileName(archFile)}" + (File.Exists(archFile) ? $"    {new FileInfo(archFile).Length / 1000:n0} kb" : "");
}