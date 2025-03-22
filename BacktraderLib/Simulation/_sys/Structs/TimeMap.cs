using BaseUtils;
using Frames;

namespace BacktraderLib._sys.Structs;


sealed record Tick(
	HashSet<string> Syms,
	string[] SymsRemoveNext
);


sealed class TimeMap
{
	readonly Tick[] ticks;

	public TimeMap(Frame<string, string>[] required)
	{
		var index = TimeMapUtils.VerifyIndicesMatch(required);
		var allSyms = TimeMapUtils.VerifySymbolsMatch(required);
		ticks = new Tick[index.Length];
		for (var t = 0; t < index.Length; t++)
		{
			var syms = allSyms.Where(e => TimeMapUtils.IsSymbolIn(e, required, t)).ToHashSet();
			var symsNext = (t < index.Length - 1) switch
			{
				false => syms,
				true => allSyms.Where(e => TimeMapUtils.IsSymbolIn(e, required, t + 1)).ToHashSet(),
			};
			var symsRemoveNext = symsNext switch
			{
				null => [],
				not null => syms.WhereA(e => !symsNext.Contains(e)),
			};
			ticks[t] = new Tick(
				syms,
				symsRemoveNext
			);
		}
	}

	public Tick this[int t] => ticks[t];
}


file static class TimeMapUtils
{
	public static bool IsSymbolIn(string sym, Frame<string, string>[] required, int t) =>
		required
			.All(e => e[sym].Values[t].IsNotNaN());

	public static DateTime[] VerifyIndicesMatch(Frame<string, string>[] required)
	{
		if (required.Length == 0) throw new ArgumentException("Impossible. required should at least contain prices");
		var index = required[0].Index;
		if (required.Any(e => !e.Index.IsSame(index))) throw new ArgumentException("Inconsistent indices");
		return index;
	}

	public static string[] VerifySymbolsMatch(Frame<string, string>[] required)
	{
		if (required.Length == 0) throw new ArgumentException("Impossible. required should at least contain prices");
		var syms = required[0].SelectA(e => e.Name);
		if (required.Any(e => !e.SelectA(f => f.Name).IsSame(syms))) throw new ArgumentException("Inconsistent symbols");
		return syms;
	}
}