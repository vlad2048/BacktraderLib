using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using BaseUtils;

namespace Feed.Trading212._sys._1_Scraping.Utils;

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
			new FieldValConverter(),
		},
	};

	static readonly JsonSerializerOptions jsonOptOptimized = new()
	{
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
		Converters =
		{
			new JsonStringEnumConverter(),
			JsonConverters_BaseUtils.Quarter,
			new NullableConverter<decimal>(),
			new FieldValConverter(),
		},
	};


	public static T LoadOr<T>(string file, T empty) => File.Exists(file) ? Load<T>(file) : empty;
	public static T LoadOrLazy<T>(string file, Func<T> empty) => File.Exists(file) ? Load<T>(file) : empty();
	public static T Load<T>(string file) => Deser<T>(File.ReadAllText(file));
	public static T Save<T>(this T obj, string file)
	{
		File.WriteAllText(file, Ser(obj));
		return obj;
	}
	public static T SaveOptimized<T>(T obj, string file)
	{
		File.WriteAllText(file, SerOptimized(obj));
		return obj;
	}
	public static string Ser<T>(this T obj) => JsonSerializer.Serialize(obj, jsonOpt);
	public static string SerOptimized<T>(T obj) => JsonSerializer.Serialize(obj, jsonOptOptimized);
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


	sealed class FieldValConverter : JsonConverter<FieldVal>
	{
		public override FieldVal? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			switch (reader.TokenType)
			{
				case JsonTokenType.Number:
				{
					var val = reader.GetDecimal();
					return new FieldVal(val, null);
				}

				case JsonTokenType.StartObject:
				{
					reader.Read();
					if (!reader.ValueTextEquals("val")) throw new ArgumentException("Invalid FieldVal (val)");
					reader.Read();
					var val = reader.GetDecimal();

					reader.Read();
					if (!reader.ValueTextEquals("ccy")) throw new ArgumentException("Invalid FieldVal (ccy)");
					reader.Read();
					var ccy = reader.GetString();

					reader.Read();

					return new FieldVal(val, ccy);
				}

				default:
					throw new ArgumentException("Invalid FieldVal");
			}
		}

		public override void Write(Utf8JsonWriter writer, FieldVal value, JsonSerializerOptions options)
		{
			switch (value.Ccy)
			{
				case null:
					writer.WriteNumberValue(value.Value);
					break;

				case not null:
					writer.WriteStartObject();
					writer.WriteNumber("val", value.Value);
					writer.WriteString("ccy", value.Ccy);
					writer.WriteEndObject();
					break;
			}
		}
	}
}