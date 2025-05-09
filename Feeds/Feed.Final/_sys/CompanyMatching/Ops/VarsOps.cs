using System.Text.RegularExpressions;

namespace Feed.Final._sys.CompanyMatching.Ops;

interface IVarsOp
{
	void Apply(string str, HashSet<string> set);
}
sealed record ReplVarsOp(string Prev, string Next) : IVarsOp
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



static class VarsOpsApplier
{
	public static string[] MakeAlternatives(this string s, IVarsOp[] ops)
	{
		var set = new HashSet<string> { s };
		foreach (var op in ops)
		{
			foreach (var elt in set.ToArray())
				op.Apply(elt, set);
		}
		return [.. set];
	}


	/*
	public static Alts[] MakeAlternatives(this IEnumerable<Norm> xs, IVarsOp[] ops, Stepper stepper)
	{
		var xsAlts = xs.Select(x => x.ApplyVarsOps(ops));

		var grps = (
				from xsAlt in xsAlts
				from name in xsAlt.Names
				select (xsAlt, name)
			)
			.GroupInDict(e => e.name, e => e.xsAlt);
		
		var dups = grps
			.WhereA(e => e.Value.Length > 1);

		stepper.Del(dups.Sum(e => e.Value.Length), "variants conflicts", () => dups.Take(10).Dump());

		return grps
			.Where(e => e.Value.Length == 1)
			.SelectManyA(e => e.Value);
	}


	static Alts ApplyVarsOps(this Norm x, IVarsOp[] ops)
	{
		var set = new HashSet<string> { x.Name };
		foreach (var op in ops)
		{
			foreach (var elt in set.ToArray())
				op.Apply(elt, set);
		}
		return new Alts(x.Refs, [.. set]);
	}
	*/
}