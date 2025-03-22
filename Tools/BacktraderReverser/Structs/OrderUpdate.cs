using BacktraderReverser.Utils;

namespace BacktraderReverser.Structs;

public enum OrderStatus
{
	Created,
	Submitted,
	Accepted,
	Partial,
	Completed,
	Canceled,
	Expired,
	Margin,
	Rejected,
}

public sealed record OrderUpdate(int Date, OrderStatus Status, OrderDir Dir, double Size, OrderType Type, double? Price, double? PriceLimit, double ExecSize, double ExecPrice, double ExecComm)
{
	public static OrderUpdate Parse(string[] xs) => new(
		xs[0].ParseDate(),
		xs[1].ParseEnum<OrderStatus>(),
		xs[2].ParseEnum<OrderDir>(),
		xs[3].ParseDouble(),
		xs[4].ParseEnum<OrderType>(),
		xs[5].ParseDoubleOpt(),
		xs[6].ParseDoubleOpt(),
		xs[7].ParseDouble(),
		xs[8].ParseDouble(),
		xs[9].ParseDouble()
	);

	public object ToDump() => new
	{
		Date,
		Status,
		Dir,
		Size,
		Type,
		Price,
		PriceLimit,
		ExecSize,
		ExecPrice,
		ExecComm,
	};
}
