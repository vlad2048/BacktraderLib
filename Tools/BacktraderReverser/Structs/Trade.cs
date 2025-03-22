using BacktraderReverser.Utils;

namespace BacktraderReverser.Structs;

public enum TradeStatus
{
	Created,
	Open,
	Closed,
}

public sealed record Trade(
	int Date,
	TradeStatus Status,
	bool IsLong,
	double Size,
	double Price,
	double Pnl,
	double Comm
)
{
	public static Trade Parse(string[] xs) => new(
		xs[0].ParseDate(),
		xs[1].ParseEnum<TradeStatus>(),
		xs[2].ParseBool(),
		xs[3].ParseDouble(),
		xs[4].ParseDouble(),
		xs[5].ParseDouble(),
		xs[6].ParseDouble()
	);

	public object ToDump() => new
	{
		Date,
		Status,
		IsLong,
		Size,
		Price,
		Pnl,
		Comm,
	};
}