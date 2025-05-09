using System.Text.Json;
using FeedUtils;

namespace Feed.NoPrefs._sys.Utils;

static class JsonUtils
{
	static readonly JsonSerializerOptions jsonOpt = new()
	{
		WriteIndented = true,
		IndentCharacter = '\t',
		IndentSize = 1,
	};

	public static T Save<T>(this T obj, string file)
	{
		Path.GetDirectoryName(file)!.CreateFolderIFN();
		var str = JsonSerializer.Serialize(obj, jsonOpt);
		File.WriteAllText(file, str);
		return obj;
	}

	public static T Load<T>(string file)
	{
		var str = File.ReadAllText(file);
		var obj = JsonSerializer.Deserialize<T>(str);
		return obj ?? throw new ArgumentException("Deserialization returned null");
	}
}