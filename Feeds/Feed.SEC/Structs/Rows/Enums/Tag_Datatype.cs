namespace Feed.SEC;

public enum Datatype
{
	Monetary,
	PerShare,
	Shares,
	Percent,
	PerUnit,
	Decimal,
	Pure,
	Integer,
	PositiveInteger,
	Memory,
	Area,
	Volume,
	Mass,
	NonNegativeInteger,
	Power,
	MonetaryPerVolume,
	Energy,
}

static class DatatypeUtils
{
	public static bool Is_Datatype(this string s) => s is "monetary" or "perShare" or "shares" or "percent" or "perUnit" or "decimal" or "pure" or "integer" or "positiveInteger" or "memory" or "area" or "volume" or "mass" or "nonNegativeInteger" or "power" or "monetaryPerVolume" or "energy";
	public static Datatype As_Datatype(this string s) => s switch
	{
		"monetary" => Datatype.Monetary,
		"perShare" => Datatype.PerShare,
		"shares" => Datatype.Shares,
		"percent" => Datatype.Percent,
		"perUnit" => Datatype.PerUnit,
		"decimal" => Datatype.Decimal,
		"pure" => Datatype.Pure,
		"integer" => Datatype.Integer,
		"positiveInteger" => Datatype.PositiveInteger,
		"memory" => Datatype.Memory,
		"area" => Datatype.Area,
		"volume" => Datatype.Volume,
		"mass" => Datatype.Mass,
		"nonNegativeInteger" => Datatype.NonNegativeInteger,
		"power" => Datatype.Power,
		"monetaryPerVolume" => Datatype.MonetaryPerVolume,
		"energy" => Datatype.Energy,
		_ => throw new ArgumentException($"Unknown Datatype: '{s}'"),
	};
	public static string Fmt_Datatype(this Datatype e) => e switch
	{
		Datatype.Monetary => "monetary",
		Datatype.PerShare => "perShare",
		Datatype.Shares => "shares",
		Datatype.Percent => "percent",
		Datatype.PerUnit => "perUnit",
		Datatype.Decimal => "decimal",
		Datatype.Pure => "pure",
		Datatype.Integer => "integer",
		Datatype.PositiveInteger => "positiveInteger",
		Datatype.Memory => "memory",
		Datatype.Area => "area",
		Datatype.Volume => "volume",
		Datatype.Mass => "mass",
		Datatype.NonNegativeInteger => "nonNegativeInteger",
		Datatype.Power => "power",
		Datatype.MonetaryPerVolume => "monetaryPerVolume",
		Datatype.Energy => "energy",
		_ => throw new ArgumentException($"Unknown Datatype: '{e}'"),
	};
}