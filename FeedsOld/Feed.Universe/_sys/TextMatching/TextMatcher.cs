using BaseUtils;
using Feed.Universe._sys.TextMatching.Structs;

namespace Feed.Universe._sys.TextMatching;




static class TextMatcher
{
	public static TextMatcherInputs Prepare(
		string[] xs,
		string[] ys,
		IVarsOp[] xVarsOps,
		INormOp[] xyNormOps,
		Func<string[], string> ysDisambiguator
	) => new(
		xs
			.ApplyNormOps(xyNormOps, null, "Sym CommonNormalization")
			.ApplyVarsOps(xVarsOps, "Sym VariantsNormalization"),
		ys
			.ApplyNormOps(xyNormOps, ysDisambiguator, "Sec CommonNormalization")
	);

	public static TextMatcherResult Run(TextMatcherInputs inputs)
	{
		var (xs, ys) = (inputs.Xs, inputs.Ys.ToList());
		var xsMatches = new Dictionary<string, TextMatch>();
		var xsUnmatched = new List<NormStrVars>();
		foreach (var x in xs)
		{
			var yMatches = ys.WhereA(y => x.StrNorms.Any(x_ => x_ == y.StrNorm));
			switch (yMatches.Length)
			{
				case 1:
					var y = yMatches.First();
					xsMatches[x.StrOrig] = new TextMatch(x, y);
					ys.Remove(y);
					break;

				default:
					xsUnmatched.Add(x);
					break;
			}
		}
		return new TextMatcherResult(
			xs,
			inputs.Ys,
			xsMatches,
			[..xsUnmatched],
			[..ys],
			inputs
		);
	}
}