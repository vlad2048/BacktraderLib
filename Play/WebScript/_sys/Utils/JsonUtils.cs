using BaseUtils;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace WebScript._sys.Utils;

static class JsonUtils
{
	static readonly JsonSerializerOptions jsonOpt = new()
	{
		WriteIndented = true,
		IndentCharacter = '\t',
		IndentSize = 1,
		Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
		Converters =
		{
			JsonConverters_BaseUtils.Quarter,
		},
	};


	public static T LoadOr<T>(string file, T empty) => File.Exists(file) ? Load<T>(file) : empty;
	public static T Load<T>(string file) => Deser<T>(File.ReadAllText(file));
	public static T Save<T>(this T obj, string file)
	{
		File.WriteAllText(file, Ser(obj));
		return obj;
	}
	public static string Ser<T>(T obj) => JsonSerializer.Serialize(obj, jsonOpt);
	public static T Deser<T>(string str) => JsonSerializer.Deserialize<T>(str, jsonOpt)!;
}