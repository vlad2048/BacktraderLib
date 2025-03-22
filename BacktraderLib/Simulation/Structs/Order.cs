using BacktraderLib._sys;

namespace BacktraderLib;


public sealed record Order(
	int Id,
	int DateCreated,
	string Symbol,
	OrderDir Dir,
	double Size,
	OrderType Type
)
{
	public object ToDump() => new
	{
		Symbol,
		Dir,
		Size = Size.FmtDumpVal(),
		Type = $"{Type}",
	};

	public override string ToString() => $"Order[id:{Id}  dateCreated:{DateCreated}  {Symbol}  {Dir}  {Size:F2}  {Type}]";
}


public enum OrderDir
{
	Buy,
	Sell,
}

public abstract record OrderType
{
	public static readonly MarketOrderType Market = new(false);
	public static readonly CloseOrderType Close = new();
	public static StopOrderType Stop(double Price) => new(Price);
	public static LimitOrderType Limit(double Price) => new(Price);
	public static StopLimitOrderType StopLimit(double Price, double PriceLimit) => new(Price, PriceLimit);
}

// @formatter:off
public sealed record MarketOrderType(bool RemovedFromUniverse) : OrderType { public override string ToString() => "Market" + (RemovedFromUniverse ? "(*)" : string.Empty); }
public sealed record CloseOrderType : OrderType { public override string ToString() => "Close"; }
public sealed record StopOrderType(double Price) : OrderType { public override string ToString() => $"Stop({Price})"; }
public sealed record LimitOrderType(double Price) : OrderType { public override string ToString() => $"Limit({Price})"; }
public sealed record StopLimitOrderType(double Price, double PriceLimit) : OrderType { public override string ToString() => $"StopLimit({Price}, {PriceLimit})"; }
// @formatter:on
