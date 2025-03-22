using System.Text.Json.Serialization;
using System.Text.Json;

namespace Feed.TwelveData._sys.JsonConverters;

sealed class CustomDateTimeConverter : JsonConverter<DateTime>
{
	public static readonly CustomDateTimeConverter Instance = new();
	public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => DateTime.Parse(reader.GetString()!);
	public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options) => writer.WriteStringValue($"{value}");
}