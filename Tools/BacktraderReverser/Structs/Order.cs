namespace BacktraderReverser.Structs;


public enum OrderDir
{
	Buy,
	Sell,
}

public enum OrderType
{
	Market,
	Close,
	Limit,
	Stop,
	StopLimit,
	StopTrail,
	StopTrailLimit,
}

public sealed record Order(int Date, OrderDir Dir, double Size, OrderType Type, double? Price, double? PriceLimit)
{
	public static Order Market(int date, OrderDir dir, double size) => new(date, dir, size, OrderType.Market, null, null);
	public static Order Close(int date, OrderDir dir, double size) => new(date, dir, size, OrderType.Close, null, null);
	public static Order Limit(int date, OrderDir dir, double size, double price) => new(date, dir, size, OrderType.Limit, price, null);
	public static Order Stop(int date, OrderDir dir, double size, double price) => new(date, dir, size, OrderType.Stop, price, null);
	public static Order StopLimit(int date, OrderDir dir, double size, double price, double priceLimit) => new(date, dir, size, OrderType.StopLimit, price, priceLimit);
	public static Order StopTrail(int date, OrderDir dir, double size, double price, double priceLimit) => new(date, dir, size, OrderType.StopTrail, price, priceLimit);
	public static Order StopTrailLimit(int date, OrderDir dir, double size, double price, double priceLimit) => new(date, dir, size, OrderType.StopTrailLimit, price, priceLimit);
}