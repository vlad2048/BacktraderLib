using BacktraderLib._sys.JsonConverters;

namespace BacktraderLib;

public sealed record Legend
{
	public double? X { get; init; }
	public double? Y { get; init; }
	public XAnchor? Xanchor { get; init; }
	public YAnchor? Yanchor { get; init; }
	public AnchorRef? Xref { get; init; }
	public AnchorRef? Yref { get; init; }
	public Orientation? Orientation { get; init; }
}

[PlotlyEnum(EnumStyle.LowerCase)]
public enum Orientation
{
	V,
	H,
}

[PlotlyEnum(EnumStyle.LowerCase)]
public enum XAnchor
{
	Auto,
	Left,
	Center,
	Right,
}

[PlotlyEnum(EnumStyle.LowerCase)]
public enum YAnchor
{
	Auto,
	Top,
	Middle,
	Bottom,
}

[PlotlyEnum(EnumStyle.LowerCase)]
public enum AnchorRef
{
	Container,
	Paper,
}