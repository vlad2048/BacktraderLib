using System.Text.Json.Serialization;
using BacktraderLib._sys.JsonConverters;

namespace BacktraderLib;

public enum MouseButton
{
	Left = 0,
	Middle = 1,
	Right = 2,
}

public sealed record MousePt(int X, int Y)
{
	public override string ToString() => $"{X},{Y}";
	public object ToDump() => ToString();
}

public sealed record Loc(
	[property: JsonConverter(typeof(StringifyConverter))] string X,
	[property: JsonConverter(typeof(StringifyConverter))] string Y,
	[property: JsonConverter(typeof(StringifyConverter))] string Z
)
{
	public override string ToString() => $"{X},{Y},{Z}";
	public object ToDump() => ToString();
}

public sealed record EvtRange(
	[property: JsonConverter(typeof(StringifyConverter))] string Min,
	[property: JsonConverter(typeof(StringifyConverter))] string Max
)
{
	public override string ToString() => $"[{Min} - {Max}]";
	public object ToDump() => ToString();
}

public sealed record EvtKeys(bool Alt, bool Ctrl, bool Meta, bool Shift)
{
	public override string ToString()
	{
		if (!Alt && !Ctrl && !Meta && !Shift) return "_";
		var list = new List<string>();
		if (Alt) list.Add("alt");
		if (Ctrl) list.Add("ctrl");
		if (Meta) list.Add("meta");
		if (Shift) list.Add("shift");
		return string.Join(",", list);
	}
	public object ToDump() => ToString();
}
