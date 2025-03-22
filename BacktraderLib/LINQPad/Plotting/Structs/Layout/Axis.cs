namespace BacktraderLib;

public sealed record Axis
{
	public bool Autorange { get; init; }
	public bool Fixedrange { get; init; }
	public FlexArray? Range { get; init; }
}