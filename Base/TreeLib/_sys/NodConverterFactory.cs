using System.Text.Json.Serialization;
using System.Text.Json;
using BaseUtils;

namespace _sys;

sealed class NodConverterFactory : JsonConverterFactory
{
	public override bool CanConvert(Type typeToConvert) =>
		typeToConvert.IsGenericType &&
		typeToConvert.GetGenericTypeDefinition() == typeof(TNod<>);

	public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
	{
		var wrappedType = typeToConvert.GetGenericArguments()[0];
		var converter = (JsonConverter)Activator.CreateInstance(typeof(NodConverter<>).MakeGenericType(wrappedType))!;
		return converter;
	}


	sealed class NodConverter<T> : JsonConverter<TNod<T>>
	{
		sealed record Ser(T V, Ser[] Kids);

		static Ser ToSer(TNod<T> nod) => new(nod.V, nod.Kids.SelectA(ToSer));
		static TNod<T> FromSer(Ser ser) => Nod.Make(ser.V, ser.Kids.SelectA(FromSer));

		public override TNod<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			using var doc = JsonDocument.ParseValue(ref reader);
			return FromSer(doc.Deserialize<Ser>(options)!);
		}

		public override void Write(Utf8JsonWriter writer, TNod<T> value, JsonSerializerOptions options) =>
			JsonSerializer.Serialize(writer, ToSer(value), options);
	}
}