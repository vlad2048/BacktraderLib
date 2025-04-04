/*
using BaseUtils;
using Feed.Trading212._sys.StateLogic.Structs;

namespace Feed.Trading212._sys.StateLogic;

static class ScrapeStateUtils
{
	public static SymbolDef[] GetSymbolsTodo(this ScrapeState state, SymbolDef[] symbols) =>
		symbols
			.WhereA(e =>
			{
				if (!state.Symbols.TryGetValue(e.SECCompany, out var symbolState)) return true;
				if (!symbolState.IsComplete) return true;
				if (DateTime.Now - symbolState.ScrapeTime >= Consts.ScrapeFreq) return true;
				return false;
			});

	public static QuarterSet GetDone(this ScrapeState state, SymbolDef symbol) =>
		state.Symbols.TryGetValue(symbol.SECCompany, out var symbolState) switch
		{
			true => symbolState.Done,
			false => new QuarterSet(),
		};


	public static void UpdateWithResult(this ScrapeState state, ScrapeResult result)
	{
		var quarterSetPrev = state.Symbols.TryGetValue(result.Symbol.SECCompany, out var symbolState) switch
		{
			true => sym
		}
	}


	static QuarterSet GetQuarterSet(this ScrapeResult result) => result.Data.Reports.ToDictionary(e => e.Key, e => e.Value.Quarters.Keys.ToHashSet());
}
*/