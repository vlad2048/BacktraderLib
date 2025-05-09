namespace Feed.Universe._sys.TextMatching.Structs;

public sealed record TextMatch(NormStrVars X, NormStr Y);

public sealed record TextMatcherResult(
	NormStrVars[] Xs,
	NormStr[] Ys,
	Dictionary<string, TextMatch> XsMatched,
	NormStrVars[] XsUnmatched,
	NormStr[] YsRemaining,
	TextMatcherInputs Inputs
)
{
	public bool IsMapped(string x) => XsMatched.ContainsKey(x);
	public string Map(string x) => XsMatched[x].Y.StrOrig;
}