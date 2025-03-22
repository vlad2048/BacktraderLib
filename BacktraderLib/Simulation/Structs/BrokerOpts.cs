namespace BacktraderLib;

public sealed record BrokerOpts
{
	public double Cash { get; init; } = 100;
	public double Comm { get; init; } = 0.0;
	public double Slippage { get; init; } = 0.0;
	public bool UseCurrentCloseForMarketOrders { get; init; } = false;
}