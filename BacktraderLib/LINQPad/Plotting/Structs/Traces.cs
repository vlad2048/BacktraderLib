using BacktraderLib._sys.JsonConverters;
using System.Text.Json.Serialization;

namespace BacktraderLib;

[JsonDerivedType(typeof(ScatterTrace))]
public interface ITrace
{
	TraceType Type { get; }
}


[PlotlyEnum(EnumStyle.PlusSeparated)]
public enum ScatterMode
{
	None,
	Lines,
	Markers,
	Text,
	LinesMarkers,
	LinesText,
	MarkersText,
	LinesMarkersText,
}

public sealed record ScatterTrace : ITrace
{
	public TraceType Type => TraceType.Scatter;
	public ScatterMode? Mode { get; init; }

	public FlexArray? X { get; init; }
	public FlexArray? Y { get; init; }

	public bool? Visible { get; init; }
	public string? Name { get; init; }
	public double? Opacity { get; init; }
	public bool? Showlegend { get; init; }
	public Marker? Marker { get; init; }
}

public sealed record BarTrace : ITrace
{
	public TraceType Type => TraceType.Bar;

	public FlexArray? X { get; init; }
	public FlexArray? Y { get; init; }
	public int? Offset { get; init; }
	public int[]? Width { get; init; }

	public bool? Visible { get; init; }
	public string? Name { get; init; }
	public double? Opacity { get; init; }
	public bool? Showlegend { get; init; }
	public Marker? Marker { get; init; }

	public string? Yaxis { get; init; }
}

public sealed record HeatmapTrace : ITrace
{
	public TraceType Type => TraceType.Heatmap;

	public FlexArray? X { get; init; }
	public FlexArray? Y { get; init; }
	public double[][]? Z { get; init; }

	public string? Name { get; init; }
	public double? Opacity { get; init; }
}



