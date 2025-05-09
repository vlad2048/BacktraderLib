using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Feed.Yahoo._sys.JsonConverters;

namespace Feed.Yahoo._sys;

static class JsonUtils
{
	static readonly JsonSerializerOptions jsonOpt = new()
	{
		WriteIndented = true,
		IndentCharacter = '\t',
		IndentSize = 1,
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		Converters =
		{
			new JsonStringEnumConverter(),
			UnixDateTimeConverter.Instance,
		},
	};

	static readonly JsonSerializerOptions jsonBeautifyOpt = new()
	{
		WriteIndented = true,
		IndentCharacter = '\t',
		IndentSize = 1,
	};


	public static string Ser<T>(this T obj) => JsonSerializer.Serialize(obj, jsonOpt);

	public static T Deser<T>(this string json) => JsonSerializer.Deserialize<T>(json, jsonOpt) ?? throw new ArgumentException("JsonSerializer.Deserialize returned null");

	public static string Beautify(this string strIn)
	{
		var obj = JsonSerializer.Deserialize<JsonNode>(strIn, jsonBeautifyOpt);
		var strOut = JsonSerializer.Serialize(obj, jsonBeautifyOpt);
		return strOut;
	}
}