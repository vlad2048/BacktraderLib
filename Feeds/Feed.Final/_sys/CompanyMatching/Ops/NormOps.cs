using System.Text.RegularExpressions;

namespace Feed.Final._sys.CompanyMatching.Ops;

interface INormOp
{
	string Apply(string str);
}
sealed record UpperNormOp : INormOp
{
	public string Apply(string str) => str.ToUpperInvariant();
}
sealed record SuffixDelOp(string Suffix) : INormOp
{
	public string Apply(string str) => str.EndsWith(Suffix) ? str[..^Suffix.Length].Trim() : str;
}
sealed record SuffixSlashDelOp : INormOp
{
	public string Apply(string str)
	{
		var lng = str.Length;
		var idx = str.LastIndexOf('/');
		if (idx == -1) return str;
		if (idx == lng - 1)
		{
			var idxPrev = str.LastIndexOf('/', idx - 1);
			if (idxPrev == -1)
				return str[..idx];
			if (lng - idxPrev <= 5 && idxPrev >= 5)
				return str[..idxPrev];
		}

		if (lng - idx <= 5 && idx >= 5)
			return str[..idx];

		return str;
	}
}
sealed record CharDelOp(char Prev) : INormOp
{
	public string Apply(string str) => str.Replace($"{Prev}", "");
}
sealed record CharReplOp(char Prev, char Next) : INormOp
{
	public string Apply(string str) => str.Replace(Prev, Next);
}
sealed record ReplNormOp(string Prev, string Next) : INormOp
{
	public string Apply(string str) => Regex.Replace(str, $@"\b{Prev}\b", Next);
}
sealed record TrimNormOp : INormOp
{
	public string Apply(string str) => str.Trim();
}
static class NormOp
{
	public static readonly INormOp Upper = new UpperNormOp();
	public static INormOp SuffixDel(string suffix) => new SuffixDelOp(suffix);
	public static readonly INormOp SuffixSlashDel = new SuffixSlashDelOp();
	public static INormOp CharDel(char prev) => new CharDelOp(prev);
	public static INormOp CharRepl(char prev, char next) => new CharReplOp(prev, next);
	public static INormOp Repl(string prev, string next) => new ReplNormOp(prev, next);
	public static readonly INormOp Trim = new TrimNormOp();
}



static class NormOpsApplier
{
	public static string Normalize(this string s, INormOp[] ops) =>
		ops.Aggregate(s, (xAcc, op) => op.Apply(xAcc));

	/*
	public static Norm[] NormalizeExtend(this string[] xs, INormOp[] ops) =>
		xs
			.Select(x => x.ApplyNormOps(ops))
			.GroupBy(e => e.Name)
			.SelectA(e => new Norm(e.SelectManyA(f => f.Refs), e.Key));

	public static Norm[] NormalizeRemove(this string[] xs, INormOp[] ops, Stepper stepper) =>
		xs
			.Select(x => x.ApplyNormOps(ops))
			.RemoveDupsBy(e => e.Name, stepper, "Normalize");

	static Norm ApplyNormOps(this string x, INormOp[] ops) => new([x], ops.Aggregate(x, (xAcc, op) => op.Apply(xAcc)));
	*/
}