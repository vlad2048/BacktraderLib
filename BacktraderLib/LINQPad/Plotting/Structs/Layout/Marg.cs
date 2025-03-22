using System.Text.Json.Serialization;

namespace BacktraderLib;

public sealed record Marg(
	[property: JsonPropertyName("l")] int Left,
	[property: JsonPropertyName("r")] int Right,
	[property: JsonPropertyName("t")] int Top,
	[property: JsonPropertyName("b")] int Bottom,
	int Pad,
	bool Autoexpand
)
{
	public static readonly Marg Empty = new(0, 0, 0, 0, 0, false);
	public static Marg All(int v) => new(v, v, v, v, 0, false);
}