using System.Reactive.Linq;
using BaseUtils;
using LINQPad;
using RxLib;

namespace ScrapeUtils;


sealed class ScriptSourceException(string Message) : Exception(Message);


public interface IScriptSource
{
	Func<DumpContainer, IObservable<string[]>> CodeUnits { get; }
	string[] Get();
}

sealed record QuerySectionScriptSource : IScriptSource
{
	internal const string SectionMarkerStart = "// [Script.Start]";
	internal const string SectionMarkerEnd = "// [Script.End]";

	public Func<DumpContainer, IObservable<string[]>> CodeUnits => logDC =>
		FileWatcher.Watch(Util.CurrentQueryPath)
			.SelectCatchToLog(logDC, _ => Get());

	public string[] Get() =>
	[
		File.ReadAllText(Util.CurrentQueryPath)
			.ExtractRelevantSection(),
	];
}


public static class ScriptSource
{
	public static readonly IScriptSource QuerySection = new QuerySectionScriptSource();
}




file static class IScriptSource_Utils
{
	public static IObservable<string[]> SelectCatchToLog<T>(this IObservable<T> source, DumpContainer logDC, Func<T, string[]> logic) =>
		source
			.Select(x =>
			{
				try
				{
					return logic(x);
				}
				catch (Exception ex)
				{
					logDC.AppendContent(ex);
					return null;
				}
			})
			.Where(e => e != null)
			.Select(e => e!)
			.ReplayD(1);


	static IObservable<T> ReplayD<T>(this IObservable<T> sourceConn, int bufferSize)
	{
		var res = sourceConn.Replay(bufferSize);
		res.Connect().D(D);
		return res;
	}
}

file static class QuerySectionScriptSource_Utils
{
	public static string ExtractRelevantSection(this string str)
	{
		var lines = str.SplitLines();
		var idxStart = lines.IdxOf(QuerySectionScriptSource.SectionMarkerStart);
		if (idxStart == -1)
			throw new ScriptSourceException($"Missing SectionMarkerStart: '{QuerySectionScriptSource.SectionMarkerStart}'");
		var idxEnd = lines.IdxOf(QuerySectionScriptSource.SectionMarkerEnd, idxStart);
		if (idxEnd == -1)
			throw new ScriptSourceException($"Missing SectionMarkerEnd: '{QuerySectionScriptSource.SectionMarkerEnd}'");
		return lines[(idxStart + 1)..(idxEnd)].JoinLines();
	}
}