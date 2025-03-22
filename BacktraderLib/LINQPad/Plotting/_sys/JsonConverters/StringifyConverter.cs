using System.Text.Json;
using System.Text.Json.Serialization;

namespace BacktraderLib._sys.JsonConverters;

public sealed class StringifyConverter : JsonConverter<string>
{
	public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions jsonOpt) =>
		reader.TokenType switch
		{
			JsonTokenType.String => reader.GetString(),
			JsonTokenType.Number => reader.GetDouble().ToString(),
			JsonTokenType.True => bool.TrueString,
			JsonTokenType.False => bool.FalseString,
			JsonTokenType.Null => "null",
			_ => reader.Fmt(),
		};


	public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions jsonOpt) => writer.WriteStringValue(value);
}



file static class TokenFmt
{
	public static string Fmt(this ref Utf8JsonReader reader)
	{
		using var doc = JsonDocument.ParseValue(ref reader);
		return doc.RootElement.GetRawText();
	}
}
