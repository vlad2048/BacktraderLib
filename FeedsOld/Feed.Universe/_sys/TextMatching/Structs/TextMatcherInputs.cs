namespace Feed.Universe._sys.TextMatching.Structs;

public sealed record TextMatcherInputs(
	NormStrVars[] Xs,
	NormStr[] Ys
);