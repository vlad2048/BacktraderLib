using System.Text.Json.Serialization;
using System.Text.Json;
using System.Reflection;
using System.Text;

namespace Play;


static class Program
{
	static void Main()
	{
	}



	static readonly JsonSerializerOptions jsonOpt = new()
	{
		WriteIndented = true,
		IndentCharacter = '\t',
		IndentSize = 1,
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		Converters = {
			new JsonStringEnumConverter(),
		},
		DefaultIgnoreCondition = JsonIgnoreCondition.Never,
		NumberHandling = JsonNumberHandling.WriteAsString,
	};
}






public class NullableConverter<T> : JsonConverter<T?> where T : struct
{
	public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions jsonOpt) =>
		reader.TokenType switch
		{
			JsonTokenType.String when reader.GetString() == "-" => null,
			_ => JsonSerializer.Deserialize<T>(ref reader, jsonOpt),
		};

	public override void Write(Utf8JsonWriter writer, T? value, JsonSerializerOptions jsonOpt) => JsonSerializer.Serialize(writer, value!.Value, jsonOpt);
}