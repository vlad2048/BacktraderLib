namespace BacktraderLib;

public static class SignExt
{
	public static double ToSign(this OrderDir dir) => dir switch
	{
		OrderDir.Buy => 1,
		OrderDir.Sell => -1,
		_ => throw new ArgumentException(),
	};

	public static double ToSign(this TradeDir dir) => dir switch
	{
		TradeDir.Long => 1,
		TradeDir.Shrt => -1,
		_ => throw new ArgumentException(),
	};
}