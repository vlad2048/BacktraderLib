using System.Text.Json.Serialization;
using BacktraderLib._sys.JsonConverters;

namespace BacktraderLib;

public sealed record Layout
{
	public int? Width { get; init; }
	public int? Height { get; init; }
	[property: JsonConverter(typeof(Write_StringToObjectConverter))]
	public string? Template { get; init; }
	public Marg? Margin { get; init; }
	public Legend? Legend { get; init; }
	public Shape[]? Shapes { get; init; }
	public Axis? Xaxis { get; init; }
};
