using System.Drawing;
using PrettyPrinting._sys;

namespace PrettyPrinting;

public interface ITxt
{
	TxtArray Txt { get; }
}

public readonly record struct Txt(string Text, Color? Back, Color? Fore)
{
	internal static readonly Txt Newline = new(Environment.NewLine, null, null);

	public static TxtArray Build(Action<ITxtBuilder> action) => TxtBuilderLogic.Build(action);
}

public sealed record TxtArray(Txt[] Array)
{
	public object ToDump() => Logger_LINQPad.Log(this);
}


public static class TxtExts
{
	public static void LogToConsole(this TxtArray xs) => Logger_Console.Log(xs);
	public static void LogToConsole(this ITxt txt) => txt.Txt.LogToConsole();
}