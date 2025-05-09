using Feed.NoPrefs._sys.DoltLogic;
using Feed.NoPrefs._sys.DoltLogic.Structs;

namespace Feed.NoPrefs._sys;

// TODO public
public static class DBs
{
	public static readonly DoltDB Rates = new(new DbNfo("post-no-preference", "rates", 3400));
	public static readonly DoltDB Stocks = new(new DbNfo("post-no-preference", "stocks", 3401));
	public static readonly DoltDB Options = new(new DbNfo("post-no-preference", "options", 3402));
	public static readonly DoltDB Earnings = new(new DbNfo("post-no-preference", "earnings", 3403));
}