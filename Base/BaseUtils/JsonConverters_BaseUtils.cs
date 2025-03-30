using System.Text.Json.Serialization;

namespace BaseUtils;

public static class JsonConverters_BaseUtils
{
	public static readonly JsonConverter Quarter = new QuarterJsonConverter();
}