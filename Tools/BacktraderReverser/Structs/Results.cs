namespace BacktraderReverser.Structs;

public sealed record Results(
	Market[] Market,
	OrderUpdate[] Orders,
	Trade[] Trades
);
