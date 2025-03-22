using System.Text.Json.Serialization;
using System.Text.Json;
using Feed.TwelveData._sys.JsonConverters;
using Feed.TwelveData._sys.Structs;

namespace Feed.TwelveData._sys;

static class JsonUtils
{
	static readonly JsonSerializerOptions jsonOpt = new()
	{
		WriteIndented = true,
		IndentCharacter = '\t',
		IndentSize = 1,
		PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
		NumberHandling = JsonNumberHandling.AllowReadingFromString,
		Converters =
		{
			new JsonStringEnumConverter(),
			MakeConverter<Plan, Access?>(PlanUtils.ToAccess, PlanUtils.ToPlan),
			CustomDateTimeConverter.Instance,
		},
	};


	public static string Ser<T>(this T obj) => JsonSerializer.Serialize(obj, jsonOpt);

	public static T Deser<T>(this string json) => JsonSerializer.Deserialize<T>(json, jsonOpt) ?? throw new ArgumentException("JsonSerializer.Deserialize returned null");



	static JsonConverter<T> MakeConverter<T, S>(Func<T, S> serFun, Func<S, T> deserFun) => new CallbackConverter<T, S>(serFun, deserFun);


	sealed class CallbackConverter<T, S>(Func<T, S> serFun, Func<S, T> deserFun) : JsonConverter<T>
	{
		public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			using var doc = JsonDocument.ParseValue(ref reader);
			return deserFun(doc.Deserialize<S>(options)!);
		}

		public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options) =>
			JsonSerializer.Serialize(writer, serFun(value), options);
	}
}