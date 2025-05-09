/*
namespace Feed.YahooOoples._sys.Structs;

enum FetchStrat
{
	/// <summary>
	/// Does not save the data
	/// </summary>
	None,

	/// <summary>
	/// - Query only Close
	/// - Unapply Splits from Close to get real Close to save
	/// - Apply Splits + Dividends to Close to return AdjustedClose to the user
	///
	/// => Unfortunately this can cause some prices to become negative when reapplying all the adjustments
	/// </summary>
	UnapplySplits,

	/// <summary>
	/// - Query both Close and AdjustedClose
	/// - The difference between them is the cumulative Dividend adjustments
	/// - Apply this difference to Open, High and Low to adjust them
	/// - Return this to the user
	///
	/// => Unfortunately with this method we never get the real Close price to serialize, so we need to query the whole date range every time
	/// </summary>
	CompareCloseAndCloseAdj,
}
*/
