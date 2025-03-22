using System.Text.Json;
using System.Text.Json.Serialization;

namespace BacktraderLib._sys.JsonConverters;

sealed class ColorConverter : JsonConverter<Color>
{
	public override Color? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
		throw new NotImplementedException("I don't think we need that");

	public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
	{
		switch (value)
		{
			case ColorRGBA e:
				writer.WriteStringValue($"#{e.R:X2}{e.G:X2}{e.B:X2}{e.A:X2}");
				break;

			default:
				throw new ArgumentException("Unknown Color type");
		}
	}
}