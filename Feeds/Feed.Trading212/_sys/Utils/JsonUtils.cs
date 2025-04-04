using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using BaseUtils;

namespace Feed.Trading212._sys.Utils;

static class JsonUtils
{
	static readonly JsonSerializerOptions jsonOpt = new()
	{
		WriteIndented = true,
		IndentCharacter = '\t',
		IndentSize = 1,
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
		Converters =
		{
			new JsonStringEnumConverter(),
			JsonConverters_BaseUtils.Quarter,
			new NullableConverter<decimal>(),
		},
	};


	public static T LoadOr<T>(string file, T empty) => File.Exists(file) ? Load<T>(file) : empty;
	public static T Load<T>(string file) => Deser<T>(File.ReadAllText(file));
	public static T Save<T>(this T obj, string file)
	{
		File.WriteAllText(file, Ser(obj));
		return obj;
	}
	public static string Ser<T>(this T obj) => JsonSerializer.Serialize(obj, jsonOpt);
	public static T Deser<T>(this string str) => JsonSerializer.Deserialize<T>(str, jsonOpt)!;


	sealed class NullableConverter<T> : JsonConverter<T?> where T : struct
	{
		public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions jsonOpt) =>
			reader.TokenType switch
			{
				JsonTokenType.String when reader.GetString() == "-" => null,
				_ => JsonSerializer.Deserialize<T>(ref reader, jsonOpt),
			};

		public override void Write(Utf8JsonWriter writer, T? value, JsonSerializerOptions jsonOpt) => JsonSerializer.Serialize(writer, value!.Value, jsonOpt);
	}
}