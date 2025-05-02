using System.Reflection;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace BacktraderLib._sys.JsonConverters;


enum EnumStyle
{
	/// <summary>
	/// 'SomeName'
	/// </summary>
	Default,

	/// <summary>
	/// 'somename'
	/// </summary>
	LowerCase,

	/// <summary>
	/// 'someName'
	/// </summary>
	CamelCase,

	/// <summary>
	/// 'some+name'
	/// </summary>
	PlusSeparated,

	/// <summary>
	/// 'some-name'
	/// </summary>
	DashSeparated,
}


sealed class PlotlyEnumAttribute(EnumStyle style) : Attribute
{
	public EnumStyle Style { get; } = style;
}



sealed class AttributeBasedEnumConverter : JsonConverterFactory
{
	static readonly JsonStringEnumConverter DefaultEnumConverter = new();
	static readonly JsonStringEnumConverter LowerCaseEnumConverter = new(new LowerCaseJsonNamingPolicy());
	static readonly JsonStringEnumConverter CamelCaseEnumConverter = new(JsonNamingPolicy.CamelCase);
	static readonly JsonStringEnumConverter PlusSeparatedEnumConverter = new(new CharSeparatedJsonNamingPolicy('+'));
	static readonly JsonStringEnumConverter DashSeparatedEnumConverter = new(new CharSeparatedJsonNamingPolicy('-'));

	public override bool CanConvert(Type typeToConvert) => typeToConvert.IsEnum;

	public override JsonConverter CreateConverter(Type enumType, JsonSerializerOptions jsonOpts)
	{
		if (!enumType.IsEnum) throw new ArgumentException("Not an Enum");
		var style = enumType.GetEnumStyle();
		var factory = style switch
		{
			EnumStyle.Default => DefaultEnumConverter,
			EnumStyle.LowerCase => LowerCaseEnumConverter,
			EnumStyle.CamelCase => CamelCaseEnumConverter,
			EnumStyle.PlusSeparated => PlusSeparatedEnumConverter,
			EnumStyle.DashSeparated => DashSeparatedEnumConverter,
			_ => throw new ArgumentException("Unknown EnumStyle"),
		};
		return factory.CreateConverter(enumType, jsonOpts);
	}


	sealed class LowerCaseJsonNamingPolicy : JsonNamingPolicy
	{
		public override string ConvertName(string name) => name.ToLowerInvariant();
	}


	sealed class CharSeparatedJsonNamingPolicy(char ch) : JsonNamingPolicy
	{
		public override string ConvertName(string s)
		{
			var list = new List<string>();
			var cur = new List<char> { s[0] };
			void Add(char c) => cur.Add(c);

			void New(char c)
			{
				if (cur.Count > 0)
				{
					list.Add(new string(cur.ToArray()));
					cur.Clear();
				}

				cur.Add(c);
			}

			foreach (var c in s.Skip(1))
			{
				if (char.IsUpper(c))
					New(c);
				else
					Add(c);
			}

			New(' ');
			return string.Join(ch, list.Select(e => e.ToLowerInvariant()));
		}
	}
}



file static class PlotlyEnumUtils
{
	public static EnumStyle GetEnumStyle(this Type enumType)
	{
		if (!enumType.IsEnum) throw new ArgumentException("Not an Enum");
		return enumType.GetCustomAttribute<PlotlyEnumAttribute>()?.Style ?? EnumStyle.Default;
	}
}

