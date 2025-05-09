using BacktraderLib;
using BaseUtils;
using Feed.Final._sys.CompanyMatching.Ops;
using Feed.Final._sys.UtilsSteppers;
using LINQPad;
using static BacktraderLib.CtrlsUtilsStatic;

namespace Feed.Final._sys.CompanyMatching;


static class CompanyMatcher
{
	public static Stepper<(Symbol, string)[]> Match(
		Symbol[] syms,
		string[] secs,
		INormOp[] normOps,
		IVarsOp[] varsOps
	)
	{
		var stepperSym = new Stepper("Symbols").Add(syms.Length, "Symbols");
		var stepperSec = new Stepper("SecCompanies").Add(secs.Length, "SecCompanies");

		var sym2norm = syms.ToDictionary(e => e, e => e.Name_TwelveData.Normalize(normOps));
		var sec2norm = secs
			.Select(e => (sec: e, norm: e.Normalize(normOps)))
			.RemoveDupsBy(e => e.norm, stepperSec, "normalization")
			.ToDictionary(e => e.sec, e => e.norm);

		var matches = new List<(Symbol, string)>();
		var unmatched = new List<Symbol>();
		var ambiguous = new List<Symbol>();

		foreach (var sym in syms)
		{
			var symAlts = sym2norm[sym].MakeAlternatives(varsOps);
			var secMatches = secs
				.Where(sec2norm.ContainsKey)
				.WhereA(sec => symAlts.Any(symAlt => sec2norm[sec] == symAlt));
			switch (secMatches.Length)
			{
				case 0:
					unmatched.Add(sym);
					break;
				
				case 1:
					matches.Add((sym, secMatches[0]));
					break;
				
				default:
					ambiguous.Add(sym);
					break;
			}
		}
		
		stepperSym.Del(unmatched.Count, "unmatched", () => horzStretch([
			unmatched.Display(),
			secs.Display(),
		]).Dump());

		stepperSym.Del(ambiguous.Count, "ambiguous", () => horzStretch([
			ambiguous.Display(),
			secs.Display(),
		]).Dump());
		
		stepperSym.Total(matches.Count);
		stepperSec.Total(sec2norm.Count);

		return matches
			.ToArray()
			.ToStepper([stepperSym, stepperSec]);
	}



	static Tag Display(this IEnumerable<Symbol> syms) => syms.ToTable(Tables.Symbols);
	static Tag Display(this string[] secs) => secs.Select(e => new Sec(e)).ToTable(Tables.Secs);
	

	sealed record Sec(string Name);



	static class Tables
	{
		public static readonly TableOptions<Symbol> Symbols = new TableOptions<Symbol>(1100, 400, TableLayout.FitData)
			.Add(e => e.Name_TwelveData)
			.Add(e => e.Name_Trading212)
			.Add(e => e.Ticker)
			.Add(e => e.Exchange)
			.Add(e => e.Cfi)
			.Add(e => e.Ccy)
			.Add(e => e.Country)
			.Search(e => e.Name_TwelveData)
			.Search(e => e.Name_Trading212)
			.Search(e => e.Ticker)
			.Search(e => e.Exchange)
			;

		public static readonly TableOptions<Sec> Secs = new TableOptions<Sec>(400, 400, TableLayout.FitData)
			.Add(e => e.Name)
			.Search(e => e.Name)
			;
	}
}