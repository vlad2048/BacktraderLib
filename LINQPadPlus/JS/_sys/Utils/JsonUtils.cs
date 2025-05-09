using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace LINQPadPlus._sys.Utils;

static class JsonUtils
{
	static readonly JsonSerializerSettings jsonOpt = new()
	{
		ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
		NullValueHandling = NullValueHandling.Ignore,
		ContractResolver = new DefaultContractResolver
		{
			NamingStrategy = new SnakeCaseNamingStrategy(),
		},
	};

	public static string Ser<T>(T obj) => JsonConvert.SerializeObject(obj, jsonOpt);
	public static T Deser<T>(string str) => JsonConvert.DeserializeObject<T>(str, jsonOpt)!;
}