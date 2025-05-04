/*
using System.Text.Json.Serialization;
using System.Text.Json;

namespace BaseUtils;

public sealed record RawJsonString(string Str);


public sealed class RawJsonStringConverter : JsonConverter<RawJsonString>
{
	public override RawJsonString? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();

	public override void Write(Utf8JsonWriter writer, RawJsonString value, JsonSerializerOptions options)
	{
		writer.WriteRawValue(value.Str, true);
	}
}
*/