using System.Buffers;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using BacktraderLib._sys.JsonConverters;
using BaseUtils;
using LINQPad.FSharpExtensions;
using RxLib;

namespace BacktraderLib._sys;


/*

Enums
=====

public enum Default { SomeName }		'SomeName'

[PlotlyEnum(EnumStyle.Default)]
public enum Default { SomeName }		'SomeName'

[PlotlyEnum(EnumStyle.LowerCase)]
public enum Default { SomeName }		'somename'

[PlotlyEnum(EnumStyle.PlusSeparated)]
public enum Default { SomeName }		'some+name'

[PlotlyEnum(EnumStyle.DashSeparated)]
public enum Default { SomeName }		'some-name'
   



Javascript				LINQPad				C#				Result
--------------------------------------------------------------------------------------------------------
{x:1,y:2}				{"x":1,"y":2}		Foo(int Y)		OK (Y=2)
{x:1,y:null}			{"x":1,"y":null}	Foo(int Y)		The JSON value could not be converted
{x:1,y:undefined}		{"x":1}				Foo(int Y)		OK (Y=0)

{x:1,y:2}				{"x":1,"y":2}		Foo(int? Y)		OK (Y=2)
{x:1,y:null}			{"x":1,"y":null}	Foo(int? Y)		OK (Y=null)
{x:1,y:undefined}		{"x":1}				Foo(int? Y)		OK (Y=null)

*/

static class PlotlyJson
{
	static readonly JsonSerializerOptions jsonOpt = MakeJsonOpt(true);
	static readonly JsonSerializerOptions jsonOptNoIndent = MakeJsonOpt(false);

	static JsonSerializerOptions MakeJsonOpt(bool indent) => new()
	{
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		WriteIndented = indent,
		IndentCharacter = '\t',
		IndentSize = 1,
		Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
		NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
		Converters =
		{
			new AttributeBasedEnumConverter(),
			new Write_FlexArrayConverter(),
			new Write_FlexValueConverter(),
			new ColorConverter(),
			new PlotlyErrorPathConverter(),
		},
		DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
		AllowTrailingCommas = true,
	};
	
	static readonly JsonWriterOptions jsonWriterOpt = new()
	{
		Indented = true,
		IndentCharacter = '\t',
		IndentSize = 1,
		Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
	};




	public static string PlotlySer<T>(this T obj) => JsonSerializer.Serialize(obj, jsonOpt);

	public static T PlotlyDeser<T>(string str)
	{
		try
		{
			return JsonSerializer.Deserialize<T>(str, jsonOpt)!;
		}
		catch (Exception ex)
		{
			throw new JsonException($"Failed to deserialize <{typeof(T).Name}>", ex);
		}
	}


	public static string PlotlyJsonMinify(this string json)
	{
		using var jDoc = JsonDocument.Parse(json);
		return JsonSerializer.Serialize(jDoc, jsonOptNoIndent);
	}


	public static string PlotlySerNested<T>(T[] arr)
	{
		var strs = arr.SelectA(e => e.PlotlySer());

		var outputBuffer = new ArrayBufferWriter<byte>();

		using (var d = new Disp("PlotlySer"))
		{
			// ReSharper disable once AccessToDisposedClosure
			var roots = strs.SelectA(e => JsonDocument.Parse(e).D(d).RootElement);
			var jsonWriter = new Utf8JsonWriter(outputBuffer, jsonWriterOpt).D(d);
			if (roots.Any(e => e.ValueKind != JsonValueKind.Object)) throw new ArgumentException("Invalid JSON object");
			jsonWriter.WriteStartObject();

			var props = roots.SelectA(root => root.EnumerateObject().ToDictionary(e => e.Name));
			var propNamesUniq = props.SelectMany(e => e.Select(f => f.Key)).Distinct().ToArray();

			foreach (var propName in propNamesUniq)
			{
				jsonWriter.WriteStartArray(propName);
				foreach (var map in props)
				{
					if (map.TryGetValue(propName, out var prop))
					{
						prop.Value.WriteTo(jsonWriter);
					}
					else
					{
						jsonWriter.WriteNullValue();
					}
				}
				jsonWriter.WriteEndArray();
			}

			jsonWriter.WriteEndObject();
		}

		return Encoding.UTF8.GetString(outputBuffer.WrittenSpan);
	}




	public static T SafeDeser<T>(string str, string role)
	{
		try
		{
			return PlotlyDeser<T>(str);
		}
		catch (Exception innerEx)
		{
			var strNice = Beautify(str);
			try
			{
				PlotlyDeser<T>(strNice);
			}
			catch (Exception innerExNice)
			{
				innerEx = innerExNice;
			}

			var ex = new JsonDeserializationException(
				strNice,
				role,
				typeof(T).Name,
				innerEx
			);
			ex.Dump();
			throw ex;
		}
	}


	static string Beautify(string strSrc)
	{
		try
		{
			var obj = PlotlyDeser<JsonNode>(strSrc);
			var strDst = obj.PlotlySer();
			return strDst;
		}
		catch (Exception)
		{
			"!!!! Error in PlotlyJson.Beautify".Dump();
			return strSrc;
		}
	}
}


public sealed class JsonDeserializationException(string inputString, string role, string typeName, Exception innerEx) : Exception(
	$"""
	 {role}
	 {new string('=', role.Length)}
	 Failed deserializing <{typeName}>
	 """,
	innerEx
)
{
	public string InputString => inputString;
}