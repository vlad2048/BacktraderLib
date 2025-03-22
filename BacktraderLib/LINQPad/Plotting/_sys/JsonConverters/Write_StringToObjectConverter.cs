using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace BacktraderLib._sys.JsonConverters;

sealed class Write_StringToObjectConverter : JsonConverter<string>
{
	static readonly JsonSerializerOptions jsonOpt = new()
	{
		WriteIndented = false,
		AllowTrailingCommas = true,
	};

	public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
		throw new NotImplementedException("We shouldn't need that");

	public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
	{
		var valueObj = JsonSerializer.Deserialize<JsonObject>(value, jsonOpt) ?? throw new ArgumentException("Ooops!");
		valueObj.WriteTo(writer, options);
	}
}