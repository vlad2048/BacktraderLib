using BacktraderLib._sys.JsonConverters;

namespace BacktraderLib;

[PlotlyEnum(EnumStyle.LowerCase)]
public enum AxisSide
{
	Clockwise,
	Counterclockwise,
	Left,
	Top,
	Bottom,
	Right,
}

public sealed record Axis
{
	public bool Autorange { get; init; }
	public bool Fixedrange { get; init; }
	public FlexArray? Range { get; init; }
	public string? Overlaying { get; init; }
	public AxisSide? Side { get; init; }
}