using BacktraderLib._sys.JsonConverters;

namespace BacktraderLib;

public sealed record Shape
{
	public Color? Fillcolor { get; init; }
	public Layer? Layer { get; init; }
	public double? Opacity { get; init; }
	public ShapeType? Type { get; init; }
	public ShapeLine? Line { get; init; }
	public FlexValue? X0 { get; init; }
	public FlexValue? X1 { get; init; }
	public FlexValue? Y0 { get; init; }
	public FlexValue? Y1 { get; init; }
	public string? Xref { get; init; }
	public string? Yref { get; init; }
}



[PlotlyEnum(EnumStyle.LowerCase)]
public enum ShapeType
{
	Circle,
	Rect,
	Path,
	Line,
}


[PlotlyEnum(EnumStyle.LowerCase)]
public enum Layer
{
	Below,
	Above,
	Between,
}

public sealed record ShapeLine
{
	public Color? Color { get; init; }
	public double? Width { get; init; }
}