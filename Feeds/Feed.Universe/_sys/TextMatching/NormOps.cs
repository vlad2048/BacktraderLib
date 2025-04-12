using System.Text.RegularExpressions;
using BaseUtils;
using Feed.Universe._sys.TextMatching.Structs;

namespace Feed.Universe._sys.TextMatching;

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
/*sealed record ReplLiteralNormOp(string Prev, string Next) : INormOp
{
	public string Apply(string str) => str.Replace(Prev, Next);
}*/
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
	//public static INormOp ReplLiteral(string prev, string next) => new ReplLiteralNormOp(prev, next);
	public static readonly INormOp Trim = new TrimNormOp();
}


interface IVarsOp
{
	void Apply(string str, HashSet<string> set);
}
sealed record ReplVarsOp(string Prev, string Next): IVarsOp
{
	public void Apply(string str, HashSet<string> set) => set.Add(Regex.Replace(str, $@"\b{Prev}\b", Next));
}
sealed record ReplLiteralVarsOp(string Prev, string Next) : IVarsOp
{
	public void Apply(string str, HashSet<string> set) => set.Add(str.Replace(Prev, Next));
}
static class VarsOp
{
	public static IVarsOp Repl(string prev, string next) => new ReplVarsOp(prev, next);
	public static IVarsOp ReplLiteral(string prev, string next) => new ReplLiteralVarsOp(prev, next);
}



static class NormOpUtils
{
	public static NormStr[] ApplyNormOps(this string[] xsOrig, INormOp[] ops, Func<string[], string>? disambiguator, string name) =>
		disambiguator switch
		{
			null => xsOrig
				.Select(x => x.ApplyNormOps(ops))
				.EnsureUniqueBy(e => e.StrNorm, $"{name} NormOp[] applying introduced duplicates"),

			not null => xsOrig
				.Select(x => x.ApplyNormOps(ops))
				.GroupBy(e => e.StrNorm)
				.Select(e => e.ToArray())
				.SelectA(e => e.Length switch
				{
					1 => e.First(),
					> 1 => disambiguator(e.SelectA(f => f.StrOrig)).ApplyNormOps(ops),
					_ => throw new ArgumentException("Impossible"),
				}),
		};

	public static NormStrVars[] ApplyVarsOps(this NormStr[] xsNorm, IVarsOp[] ops, string name)
	{
		var xsVars = xsNorm.SelectA(x => x.ApplyVarsOps(ops));
		xsVars.SelectMany(e => e.StrNorms).EnsureUniqueBy(e => e, $"{name} applying VarsOp[] introduced duplicates");
		return xsVars;
	}

	public static NormStr ApplyNormOps(this string x, INormOp[] ops) => new(
		ops.Aggregate(x, (xAcc, op) => op.Apply(xAcc)),
		x
	);

	public static NormStrVars ApplyVarsOps(this NormStr x, IVarsOp[] ops)
	{
		var set = new HashSet<string> { x.StrNorm };
		foreach (var op in ops)
		{
			foreach (var elt in set.ToArray())
				op.Apply(elt, set);
		}

		return new(
			[..set],
			x.StrOrig
		);
	}
}