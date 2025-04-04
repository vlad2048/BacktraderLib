using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace BaseUtils;

public enum QNum
{
	Q1,
	Q2,
	Q3,
	Q4,
}

public sealed record Quarter(int Year, QNum Q) : IComparable<Quarter>
{
	public static readonly Quarter MinValue = new(2001, QNum.Q1);
	public static readonly Quarter MaxValue = new(2025, QNum.Q4);
	public static readonly Quarter[] All = MakeAll();

	public override string ToString() => $"{Q}'{$"{Year}"[2..]}";
	public object ToDump() => ToString();


	// Operators
	// =========
	public static Quarter operator +(Quarter q, int v)
	{
		if (v > 0)
			for (var i = 0; i < v; i++)
				q++;
		else if (v < 0)
			for (var i = 0; i < -v; i++)
				q--;
		return q;
	}
	public static Quarter operator -(Quarter q, int v)
	{
		if (v > 0)
			for (var i = 0; i < v; i++)
				q--;
		else if (v < 0)
			for (var i = 0; i < -v; i++)
				q++;
		return q;
	}
	public static Quarter operator ++(Quarter q) =>
		q.Q switch
		{
			QNum.Q4 => new Quarter(q.Year + 1, QNum.Q1),
			_ => q with { Q = q.Q + 1 },
		};
	public static Quarter operator --(Quarter q) =>
		q.Q switch
		{
			QNum.Q1 => new Quarter(q.Year - 1, QNum.Q4),
			_ => q with { Q = q.Q - 1 },
		};


	// IComparable<Quarter>
	// ====================
	public int CompareTo(Quarter? other)
	{
		if (ReferenceEquals(this, other)) return 0;
		if (ReferenceEquals(null, other)) return 1;
		var yearComparison = Year.CompareTo(other.Year);
		if (yearComparison != 0) return yearComparison;
		return Q.CompareTo(other.Q);
	}
	public static bool operator <(Quarter? left, Quarter? right) => Comparer<Quarter>.Default.Compare(left, right) < 0;
	public static bool operator >(Quarter? left, Quarter? right) => Comparer<Quarter>.Default.Compare(left, right) > 0;
	public static bool operator <=(Quarter? left, Quarter? right) => Comparer<Quarter>.Default.Compare(left, right) <= 0;
	public static bool operator >=(Quarter? left, Quarter? right) => Comparer<Quarter>.Default.Compare(left, right) >= 0;


	
	// Parsers
	// =======

	// (2017, Q1|Q2|Q3|FY)
	public static bool CanParseSubRowQuarter(string fy, string fp) =>
		int.TryParse(fy, out _) &&
		fp is "Q1" or "Q2" or "Q3" or "FY";

	public static Quarter ParseSubRowQuarter(int fy, string fp) => new(
		fy,
		fp switch
		{
			"Q1" => QNum.Q1,
			"Q2" => QNum.Q2,
			"Q3" => QNum.Q3,
			"FY" => QNum.Q4,
			_ => throw new ArgumentException("This should be impossible"),
		}
	);

	// Q3'17
	public static Quarter ParseScrapeQuarter(string str)
	{
		var match = Regex.Match(str, @"Q([1-4])'(\d{2})");
		if (!match.Success) throw new ArgumentException($"Failed to extract Quarter from '{str}'");
		var quarterNumber = int.Parse(match.Groups[1].Value);
		var yearShort = int.Parse(match.Groups[2].Value);

		var year = yearShort switch
		{
			>= 50 => yearShort + 1900,
			_ => yearShort + 2000,
		};

		return new Quarter(year, (QNum)(quarterNumber - 1));
	}



	// Private
	// =======
	static Quarter[] MakeAll()
	{
		var list = new List<Quarter>();
		for (var q = MinValue; q <= MaxValue; q++)
			list.Add(q);
		return [.. list];
	}
}




public static class QuarterUtils
{
	public static int Distance(this Quarter qA, Quarter qB)
	{
		var vA = qA.Year * 4 + (int)qA.Q;
		var vB = qB.Year * 4 + (int)qB.Q;
		return Math.Abs(vA - vB);
	}
}




public sealed class QuarterJsonConverter : JsonConverter<Quarter>
{
	public override Quarter Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var str = reader.GetString() ?? throw new ArgumentException("Impossible");
		return FromStr(str);
	}

	public override void Write(Utf8JsonWriter writer, Quarter value, JsonSerializerOptions options)
	{
		JsonSerializer.Serialize(writer, ToStr(value), options);
	}

	public override Quarter ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var str = reader.GetString() ?? throw new ArgumentException("Impossible");
		return FromStr(str);
	}

	public override void WriteAsPropertyName(Utf8JsonWriter writer, Quarter value, JsonSerializerOptions options)
	{
		var propertyName = ToStr(value);
		writer.WritePropertyName(propertyName);
	}


	static string ToStr(Quarter e) => $"{e.Year}-{e.Q}";
	static Quarter FromStr(string s)
	{
		if (s.Length != 7) throw new ArgumentException($"Cannot deserialize Quarter (wrong length): '{s}'");
		if (!int.TryParse(s[..4], out var year)) throw new ArgumentException($"Cannot deserialize Quarter (wrong Year): '{s}'");
		if (!int.TryParse(s[6..], out var q)) throw new ArgumentException($"Cannot deserialize Quarter (wrong Q): '{s}'");
		if (q < 1 || q > 4) throw new ArgumentException($"Cannot deserialize Quarter (wrong Q value): '{s}'");
		return new Quarter(year, (QNum)(q - 1));
	}
}
