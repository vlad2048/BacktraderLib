using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using BaseUtils;

namespace FeedUtils;

public class Jsoner
{
	public static readonly Jsoner I = new();

	protected virtual JsonSerializerOptions Opt => new()
	{
		WriteIndented = true,
		IndentCharacter = '\t',
		IndentSize = 1,
		Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
		Converters =
		{
			new JsonStringEnumConverter(),
			new QuarterJsonConverter(),
		},
	};

	Jsoner()
	{
	}

	public string Ser<T>(T obj) => JsonSerializer.Serialize(obj, Opt);
	public T Deser<T>(string str) => JsonSerializer.Deserialize<T>(str, Opt) ?? throw new ArgumentException("Deserialization returned null");
	public T Save<T>(T obj, string file) => obj.With(() => File.WriteAllText(file, Ser(obj)));
	public T Load<T>(string file) => Deser<T>(File.ReadAllText(file));
}
