using System.Text.Json.Serialization;
using System.Text.Json;
using Feed.Yahoo._sys.Utils;

namespace Feed.Yahoo._sys.JsonConverters;

sealed class UnixDateTimeConverter : JsonConverter<DateTime>
{
	public static readonly UnixDateTimeConverter Instance = new();
	public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => reader.GetInt64().ToDateTime();
	public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options) => writer.WriteNumberValue(value.ToUnixTime());
}