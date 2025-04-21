using BacktraderLib._sys.JsonConverters;
using System.Text.Json;

namespace BacktraderLib._sys.Utils;

static class JsonEnumUtils
{
	static readonly JsonSerializerOptions jsonOpt = new()
	{
		WriteIndented = true,
		IndentCharacter = '\t',
		IndentSize = 1,
		Converters =
		{
			new AttributeBasedEnumConverter(),
		},
	};

	public static string Ser<T>(T obj) => JsonSerializer.Serialize(obj, jsonOpt);
}