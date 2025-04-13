using Feed.SEC._sys.Utils;

namespace Feed.SEC;

public sealed record NumPreXRef(
	string Adsh,
	string Tag,
	string Version
)
{
	public override string ToString() => $"[{Adsh} + {$"{Tag}".Ellipse(Consts.DumpCaps.TagInNumPreXRef)}]";
	public object ToDump() => $"{this}";
}