namespace BacktraderLib;

public sealed record Config
{
	/// <summary>
	/// Disable all interactions and do not display any buttons
	/// </summary>
	public bool? StaticPlot { get; init; }

	/// <summary>
	/// Show 'Edit Chart' button
	/// </summary>
	public bool? ShowEditInChartStudio { get; init; }
}
