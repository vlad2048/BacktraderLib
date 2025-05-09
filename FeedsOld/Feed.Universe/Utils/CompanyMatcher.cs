using BaseUtils;
using Feed.Universe._sys;
using Feed.Universe._sys.TextMatching;
using Feed.Universe._sys.TextMatching.Structs;

namespace Feed.Universe;


public static class CompanyMatcher
{
	static readonly HashSet<string> secNameIgnores =
	[
		"NORTHERN STATES POWER CO",
		"NORTHERN STATES POWER CO /WI/",
	];

	public static string[] GetSymNames() =>
		Enum.GetValues<ExchangeName>()
			.SelectMany(e => UniverseConstituentCleaner.GetSymbolsInternal(Universe.Exchange(e)))
			.SelectDistinctA(e => e.Item2.Name);

	public static (string[], Func<string[], string>) GetSecNamesAndDisambiguator(string? investigateName = null)
	{
		var changeData = SEC.API.Utils.GetNameChanges();
		var formerSet = changeData.Changes.ToHashSet(e => e.Former);
		var cutoff = changeData.LastFiledDates.Values.Max().AddDays(-30 * 6);

		var allNames = SEC.API.Utils.GetCompanies();

		var names = allNames
			.WhereInvestigate(e => !secNameIgnores.Contains(e), investigateName, "InIgnores")
			.WhereInvestigate(e => changeData.LastFiledDates[e] >= cutoff, investigateName, "LastFiledDate")
			.WhereInvestigate(e => !formerSet.Contains(e), investigateName, "FormerSet")
			.ToArray();

		string Disambiguate(string[] arr)
		{
			if (arr.Length < 1) throw new ArgumentException("Impossible");
			var maxT = arr.Max(e => changeData.LastFiledDates[e]);
			var matches = changeData.LastFiledDates.WhereA(kv => arr.Contains(kv.Key) && kv.Value == maxT);
			switch (matches.Length)
			{
				case 1:
					return matches[0].Key;

				default:
					throw new ArgumentException($"Disambiguation failed: {matches.Length} matches for {arr.Length} names: {arr.JoinText("; ")}");
			}
		}

		return (names, Disambiguate);
	}


	public static TextMatcherResult Match(
		string[] symNames,
		string[] secNames,
		Func<string[], string> secDisambiguator
	) =>
		TextMatcher.Run(TextMatcher.Prepare(
			symNames,
			secNames,
			xVarsOps,
			xyNormOps,
			secDisambiguator
		));





	static IEnumerable<string> WhereInvestigate(this IEnumerable<string> source, Func<string, bool> predicate, string? investigateName, string title) =>
		source
			.Where(e =>
			{
				var res = predicate(e);
				if (!res && e == investigateName)
					Console.WriteLine($"[Investigate '{investigateName}'] Removed because of {title}");
				return res;
			});





	static readonly INormOp[] xyNormOps =
	[
		NormOp.Upper,

		// Remove from SecNames (these suffixes are only in SecNames)
		NormOp.SuffixSlashDel,

		NormOp.Repl(".COM", ""),

		// Remove from SymNames (these chars are only in SymNames)
		NormOp.CharDel('%'),
		NormOp.CharDel('+'),
		NormOp.CharDel('?'),
		NormOp.CharRepl('é', 'E'),
		NormOp.CharRepl('–', ' '),

		// Remove difficulties
		NormOp.CharDel(','),
		NormOp.CharDel('.'),
		NormOp.CharRepl('-', ' '),
		NormOp.CharRepl('/', ' '),

		// Replace these words in SymNames (these are only in SymNames)
		NormOp.Repl("PUBLIC LIMITED COMPANY", "PLC"),
		NormOp.Repl("AND", "&"),

		NormOp.Trim,
	];

	static readonly IVarsOp[] xVarsOps =
	[
		VarsOp.Repl("CORPORATION", "CORP"),
		VarsOp.Repl("COMPANY", "CO"),
		VarsOp.Repl("INCORPORATED", "INC"),
		VarsOp.Repl("MANAGEMENT", "INC"),
		VarsOp.ReplLiteral("O'", "O "),
		VarsOp.ReplLiteral("'", ""),
		VarsOp.ReplLiteral("THE ", ""),
	];
}