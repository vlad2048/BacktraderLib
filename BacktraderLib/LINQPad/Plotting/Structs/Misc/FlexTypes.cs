using BaseUtils;

namespace BacktraderLib;

using D = double;
using I = int;
using T = DateTime;
using S = string;

enum FlexType
{
	Dbl,
	Int,
	Dat,
	Str,
}


public sealed class FlexArray
{
	internal FlexType Type { get; init; }
	internal D[] ArrD { get; init; } = default!;
	internal I[] ArrI { get; init; } = default!;
	internal T[] ArrT { get; init; } = default!;
	internal S[] ArrS { get; init; } = default!;

	public static implicit operator FlexArray(D[] xs) => new() { Type = FlexType.Dbl, ArrD = xs };
	public static implicit operator FlexArray(I[] xs) => new() { Type = FlexType.Int, ArrI = xs };
	public static implicit operator FlexArray(T[] xs) => new() { Type = FlexType.Dat, ArrT = xs };
	public static implicit operator FlexArray(S[] xs) => new() { Type = FlexType.Str, ArrS = xs };

	public override S ToString() => $"FlexArray Type:{Type}";

	public object ToDump() => Type switch
	{
		FlexType.Dbl => ArrD,
		FlexType.Int => ArrI,
		FlexType.Dat => ArrT,
		FlexType.Str => ArrS,
		_ => throw new ArgumentException("Unknown FlexType"),
	};

	internal static FlexArray FromValues(FlexValue[] vals)
	{
		if (vals.Length == 0) throw new ArgumentException("Empty");
		var t = vals[0].Type;
		if (vals.Any(e => e.Type != t)) throw new ArgumentException("Inconsistent types");
		return t switch
		{
			FlexType.Dbl => vals.SelectA(e => e.ValD),
			FlexType.Int => vals.SelectA(e => e.ValI),
			FlexType.Dat => vals.SelectA(e => e.ValT),
			FlexType.Str => vals.SelectA(e => e.ValS),
			_ => throw new ArgumentException("Unknown FlexType"),
		};
	}
}


public sealed class FlexValue
{
	internal FlexType Type { get; init; }
	internal D ValD { get; init; } = default!;
	internal I ValI { get; init; } = default!;
	internal T ValT { get; init; } = default!;
	internal S ValS { get; init; } = default!;

	public static implicit operator FlexValue(D xs) => new() { Type = FlexType.Dbl, ValD = xs };
	public static implicit operator FlexValue(I xs) => new() { Type = FlexType.Int, ValI = xs };
	public static implicit operator FlexValue(T xs) => new() { Type = FlexType.Dat, ValT = xs };
	public static implicit operator FlexValue(S xs) => new() { Type = FlexType.Str, ValS = xs };

	public override S ToString() => $"FlexValue Type:{Type}";

	public object ToDump() => Type switch
	{
		FlexType.Dbl => ValD,
		FlexType.Int => ValI,
		FlexType.Dat => ValT,
		FlexType.Str => ValS,
		_ => throw new ArgumentException("Unknown FlexType"),
	};
}

