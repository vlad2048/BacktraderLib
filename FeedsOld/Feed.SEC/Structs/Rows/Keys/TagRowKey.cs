using Feed.SEC._sys.Utils;

namespace Feed.SEC;

public sealed record TagRowKey(
	string Tag,
	string Version
)
{
	public override string ToString() => $"{Tag}@{FmtVersion(Version)}".Ellipse(Consts.DumpCaps.Tag);
	public object ToDump() => $"{this}";


	const string OfficialVersionPrefix = "us-gaap/";

	static string FmtVersion(string version) =>
		version.StartsWith(OfficialVersionPrefix) switch
		{
			true => $"({version[OfficialVersionPrefix.Length..]})",
			false => version,
		};
}