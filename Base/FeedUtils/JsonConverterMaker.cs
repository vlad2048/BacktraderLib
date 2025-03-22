using System.Text.Json.Serialization;
using System.Text.Json;

namespace FeedUtils;

public static class JsonConverterMaker
{
	public static JsonConverter<T> Make<T, S>(Func<T, S> serFun, Func<S, T> deserFun) => new CallbackConverter<T, S>(serFun, deserFun);


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