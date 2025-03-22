using System.Text.Json;
using System.Text.Json.Serialization;

namespace BacktraderLib._sys.JsonConverters;

sealed class Write_FlexArrayConverter : JsonConverter<FlexArray>
{
	public override FlexArray Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions jsonOpt) => throw new NotImplementedException();

	public override void Write(Utf8JsonWriter writer, FlexArray value, JsonSerializerOptions jsonOpts)
	{
		switch (value.Type)
		{
			case FlexType.Dbl:
				writer.Write(value.ArrD, jsonOpts);
				break;
			case FlexType.Int:
				writer.Write(value.ArrI, jsonOpts);
				break;
			case FlexType.Dat:
				writer.Write(value.ArrT, jsonOpts);
				break;
			case FlexType.Str:
				writer.Write(value.ArrS, jsonOpts);
				break;
			default:
				throw new ArgumentException("Unknown FlexType");
		}
	}
}


sealed class Write_FlexValueConverter : JsonConverter<FlexValue>
{
	public override FlexValue Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions jsonOpt) => throw new NotImplementedException();

	public override void Write(Utf8JsonWriter writer, FlexValue value, JsonSerializerOptions jsonOpts)
	{
		switch (value.Type)
		{
			case FlexType.Dbl:
				writer.Write(value.ValD, jsonOpts);
				break;
			case FlexType.Int:
				writer.Write(value.ValI, jsonOpts);
				break;
			case FlexType.Dat:
				writer.Write(value.ValT, jsonOpts);
				break;
			case FlexType.Str:
				writer.Write(value.ValS, jsonOpts);
				break;
			default:
				throw new ArgumentException("Unknown FlexType");
		}
	}
}



file static class ConverterUtils
{
	public static void Write<T>(this Utf8JsonWriter writer, T obj, JsonSerializerOptions jsonOpts) => ((JsonConverter<T>)jsonOpts.GetConverter(typeof(T))).Write(writer, obj, jsonOpts);
}