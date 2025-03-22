using BacktraderReverser.Utils;

namespace BacktraderReverser.Structs;

public sealed record Market(int Date, double Open, double High, double Low, double Close, double Equity, double Cash)
{
	public static Market Parse(string[] xs) => new(
		xs[0].ParseDate(),
		xs[1].ParseDouble(),
		xs[2].ParseDouble(),
		xs[3].ParseDouble(),
		xs[4].ParseDouble(),
		xs[5].ParseDouble(),
		xs[6].ParseDouble()
	);

	public object ToDump() => new
	{
		Date,
		Open,
		High,
		Low,
		Close,
		Equity,
		Cash,
	};
}