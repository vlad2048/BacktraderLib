using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FeedUtils;

public static class JsonUtilsGeneric
{
	public static readonly JsonSerializerOptions Opt = new()
	{
		WriteIndented = true,
		IndentCharacter = '\t',
		IndentSize = 1,
		Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
		Converters =
		{
			new JsonStringEnumConverter(),
		},
	};

	public static string Ser<T>(T obj) => JsonSerializer.Serialize(obj, Opt);
	public static T Deser<T>(string str) => JsonSerializer.Deserialize<T>(str, Opt) ?? throw new ArgumentException("Deserialization returned null");

	public static T Save<T>(T obj, string file)
	{
		File.WriteAllText(file, Ser(obj));
		return obj;
	}
	public static T Load<T>(string file) => Deser<T>(File.ReadAllText(file));
}