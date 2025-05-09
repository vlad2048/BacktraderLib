using System.Text.Encodings.Web;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BacktraderLib._sys.Utils;

static class TableJsonUtils
{
	static readonly HashSet<string> rawFieldNames =
	[
		"formatter",
		"dataFiltered",
		"dataLoaded",
	];

	public static JsonArray ToJsonArray(this IEnumerable<JsonObject> items) => new(items.OfType<JsonNode?>().ToArray());
	public static JsonArray ToJsonArray<T>(this IEnumerable<T> items) => items.Ser().Deser<JsonArray>();

	public static JsonObject ToJsonObject(this IEnumerable<KeyValuePair<string, JsonNode?>> items) => new(items.ToArray());
	public static JsonObject ToJsonObjectGen<T>(this T obj) => obj.Ser().Deser<JsonObject>();

	public static JsonObject Merge(this IEnumerable<JsonObject> objs) =>
		new(
			from obj in objs
			from kv in obj
			select new KeyValuePair<string, JsonNode>(
				kv.Key,
				kv.Value.Ser().Deser<JsonNode>()
			)
		);

	public static KeyValuePair<string, JsonNode?> KeyVal<T>(string key, T val) => new(
		key,
		JsonValue.Create(val)
	);

	public static string SerEnum<E>(this E val) where E : struct, Enum => JsonEnumUtils.Ser(val).RemoveEnclosingDoubleQuotes();

	public static string? SerEnum<E>(this E? val) where E : struct, Enum => val.HasValue switch
	{
		true => val.Value.SerEnum(),
		false => null,
	};

	public static string Ser<T>(this T obj) => JsonSerializer.Serialize(obj, jsonOpt);
	public static string SerFinal<T>(this T obj) => JsonSerializer.Serialize(obj, jsonOptFinal);

	static T Deser<T>(this string str) => JsonSerializer.Deserialize<T>(str, jsonOpt)!;



	static string RemoveEnclosingDoubleQuotes(this string s)
	{
		if (s.Length < 2) return s;
		if (s[0] == '"' && s[^1] == '"') return s[1..^1];
		return s;
	}


	static readonly JsonSerializerOptions jsonOpt = new()
	{
		WriteIndented = true,
		IndentCharacter = '\t',
		IndentSize = 1,
		Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
		NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
		DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
	};

	static readonly JsonSerializerOptions jsonOptFinal = new(jsonOpt)
	{
		Converters =
		{
			new FormatterFixConverter(),
		},
	};




	sealed class FormatterFixConverter : JsonConverter<JsonObject>
	{
		public override JsonObject Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException("Expected StartObject token");
			if (JsonNode.Parse(ref reader) is not JsonObject jsonObject) throw new JsonException("Failed to deserialize JsonObject");
			return jsonObject;
		}

		public override void Write(Utf8JsonWriter writer, JsonObject value, JsonSerializerOptions options)
		{
			writer.WriteStartObject();
			foreach (var property in value)
			{
				writer.WritePropertyName(property.Key);

				switch (property)
				{
					case { Value: not null, Key: var key } when rawFieldNames.Contains(key):
						var str = property.Value.ToString();
						writer.WriteRawValue(str, true);
						break;

					case { Value: JsonObject nestedObject }:
						JsonSerializer.Serialize(writer, nestedObject, options);
						break;

					case { Value: JsonArray nestedArray }:
						writer.WriteStartArray();
						foreach (var item in nestedArray)
						{
							if (item is JsonObject arrayNestedObject)
							{
								JsonSerializer.Serialize(writer, arrayNestedObject, options);
							}
							else
							{
								JsonSerializer.Serialize(writer, item, options);
							}
						}
						writer.WriteEndArray();
						break;

					default:
						JsonSerializer.Serialize(writer, property.Value, options);
						break;
				}
			}
			writer.WriteEndObject();
		}
	}
}