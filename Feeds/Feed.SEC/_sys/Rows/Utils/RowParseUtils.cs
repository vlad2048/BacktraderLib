using System.Globalization;

namespace Feed.SEC._sys.Rows.Utils;

static class RowParseUtils
{
	public static bool Is_DateOnlyCompact(this string s) => DateOnly.TryParseExact(s, "yyyyMMdd", null, DateTimeStyles.None, out _);
	public static DateOnly As_DateOnlyCompact(this string s) => DateOnly.ParseExact(s, "yyyyMMdd", null);
	public static string Fmt_DateOnlyCompact(this DateOnly s) => $"{s:yyyyMMdd}";

	public static bool Is_NullableDateOnlyCompact(this string s) => s == string.Empty || s.Is_DateOnlyCompact();
	public static DateOnly? As_NullableDateOnlyCompact(this string s) => s == string.Empty ? null : s.As_DateOnlyCompact();
	public static string Fmt_NullableDateOnlyCompact(this DateOnly? s) => s switch
	{
		null => string.Empty,
		not null => $"{s:yyyyMMdd}",
	};

	public static bool Is_DateTime(this string s) => DateTime.TryParse(s, out _);
	public static DateTime As_DateTime(this string s) => DateTime.Parse(s);
	public static string Fmt_DateTime(this DateTime s) => $"{s:yyyy-MM-dd HH:mm:ss}";

	public static bool Is_Int(this string s) => int.TryParse(s, out _);
	public static int As_Int(this string s) => int.Parse(s);
	public static string Fmt_Int(this int s) => $"{s}";

	public static bool Is_Bool(this string s) => s is "0" or "1";
	public static bool As_Bool(this string s) => s switch
	{
		"0" => false,
		"1" => true,
		_ => throw new ArgumentException($"Not a recognized 0,1 bool: '{s}'"),
	};
	public static string Fmt_Bool(this bool s) => s switch
	{
		false => "0",
		true => "1",
	};

	public static bool Is_BoolInstant(this string s) => s is "D" or "I";
	public static bool As_BoolInstant(this string s) => s switch
	{
		"D" => false,
		"I" => true,
		_ => throw new ArgumentException($"Not a recognized D,I bool: '{s}'"),
	};
	public static string Fmt_BoolInstant(this bool s) => s switch
	{
		false => "D",
		true => "I",
	};

	public static bool Is_String(this string s) => s != string.Empty;
	public static string As_String(this string s) => s;
	public static string Fmt_String(this string s) => s;

	public static bool Is_NullableString(this string s) => true;
	public static string? As_NullableString(this string s) => s == string.Empty ? null : s;
	public static string Fmt_NullableString(this string? s) => s ?? string.Empty;

	public static bool Is_NullableDecimal(this string s) => s == string.Empty || decimal.TryParse(s, out _);
	public static decimal? As_NullableDecimal(this string s) => s switch
	{
		"" => null,
		_ => decimal.Parse(s),
	};
	public static string Fmt_NullableDecimal(this decimal? s) => s switch
	{
		not null => $"{s}",
		null => string.Empty,
	};
}
