using BacktraderLib._sys;
using BaseUtils;
using Frames;

namespace BacktraderLib;


public sealed record ResultSet(
	string Symbol,
	OrderUpdate[] Orders,
	Trade[] Trades
)
{
	public int TradesWin => Trades.Count(e => e.Pnl >= 0);
	public int TradesLos => Trades.Count(e => e.Pnl < 0);
	public double Profit { get; } = Trades.Sum(e => e.Pnl);
}


public sealed record SimResult(
	int TimeOfs,
	double CashStart,
	Serie<string> Equity,
	ResultSet All,
	IReadOnlyDictionary<string, ResultSet> Syms
)
{
	public double Return { get; } = Equity.Values[^1] / Equity.Values[TimeOfs] - 1;
}

public interface IWithSimResult
{
	SimResult SimResult { get; }
}



public interface ICtx
{
	IObservable<int> WhenTick { get; }

	double Cash { get; }
	HashSet<string> Syms { get; }
	IReadOnlyDictionary<string, double> Positions { get; }
	IReadOnlyDictionary<string, bool> IsReady { get; }
	IObservable<OrderUpdate> WhenOrder { get; }
	Order PlaceOrder(OrderDef def);
}

public static class ICtxExt
{
	public static Order Buy(this ICtx ctx, string symbol, double size, OrderType type) => ctx.PlaceOrder(new OrderDef(symbol, OrderDir.Buy, size, type));
	public static Order Sell(this ICtx ctx, string symbol, double size, OrderType type) => ctx.PlaceOrder(new OrderDef(symbol, OrderDir.Sell, size, type));
}



public delegate void Strat(ICtx ctx);



public static class Sim
{
	public static SimResult Run(
		Frame<string, string, Bar> prices,
		int timeOfs,
		BrokerOpts brokerOpts,
		Frame<string, string>[] required,
		Strat strat
	)
	{
		if (timeOfs >= prices.RowCount) throw new ArgumentException("timeOfs is too high");

		using var broker = new Broker(prices, brokerOpts, required);
		using var ctx = new Ctx(broker);
		var orders = new List<OrderUpdate>();
		broker.WhenOrder.Subscribe(orders.Add);

		strat(ctx);

		for (var t = timeOfs; t < prices.RowCount; t++)
		{
			broker.TickStart(t);

			ctx.Tick(t);

			broker.TickEnd();
		}

		var all = new ResultSet(
			"All",
			[.. orders],
			TradeCalculator.Calc(orders, prices.RowCount - 1, prices.ToDictionary(e => e.Name, e => e[Bar.Close].Values[^1]))
		);
		var symbols = all.SplitSymbols(prices.SelectA(e => e.Name));

		return new SimResult(
			timeOfs,
			brokerOpts.Cash,
			broker.Equity,
			all,
			symbols
		);
	}


	sealed class Ctx(Broker broker) : ICtx, IDisposable
	{
		public void Dispose() => whenTick.OnCompleted();

		readonly Subject<int> whenTick = new();


		public IObservable<int> WhenTick => whenTick.AsObservable();

		public double Cash => broker.Cash;
		public HashSet<string> Syms => broker.Syms;
		public IReadOnlyDictionary<string, double> Positions => broker.Positions;
		public IReadOnlyDictionary<string, bool> IsReady => broker.IsReady;
		public IObservable<OrderUpdate> WhenOrder => broker.WhenOrder;
		public Order PlaceOrder(OrderDef def) => broker.PlaceOrder(def);


		public void Tick(int t) => whenTick.OnNext(t);
	}



	static IReadOnlyDictionary<string, ResultSet> SplitSymbols(this ResultSet all, string[] symbols) =>
		symbols
			.ToOrderedDictionary(
				e => e,
				e =>
				{
					var orders = all.Orders.WhereA(f => f.Order.Symbol == e);
					var trades = all.Trades.WhereA(f => f.Symbol == e);
					return new ResultSet(
						e,
						orders,
						trades.CheckTradesAssumptions()
					);
				});


	static Trade[] CheckTradesAssumptions(this Trade[] trades)
	{
		if (trades.Any(e => e.EntrDate == e.ExitDate && !e.IsOpen)) throw new ArgumentException("We shouldn't have closed trades entering and exiting on the same tick"); 
		if (trades.Zip(trades.Skip(1)).Any(t => t.First.ExitDate > t.Second.EntrDate)) throw new ArgumentException("We shouldn't have overlapping or non-chronological trades");
		return trades;
	}
}

