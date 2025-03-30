using _sys;
using System.Text.Json.Serialization;

public static class JsonConverters_TreeLib
{
	public static readonly JsonConverter NodConverterFactory = new NodConverterFactory();
}