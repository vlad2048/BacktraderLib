using BacktraderLib._sys.Structs;
using BaseUtils;
using Frames;

namespace BacktraderLib._sys;


sealed class Broker : IDisposable
{
	sealed record PendingOrder(Order Order)
	{
		public bool IsTriggered { get; set; }
	}

	public void Dispose() => whenOrder.OnCompleted();


	readonly Frame<string, string, Bar> prices;
	readonly BrokerOpts opts;
	readonly TimeMap timeMap;
	readonly List<PendingOrder> pending = [];
	readonly Dictionary<string, double> positions;
	readonly Dictionary<string, bool> isReady;
	readonly double[] equity;
	readonly Subject<OrderUpdate> whenOrder = new();

	int time;
	int orderIdCounter;

	public double Cash { get; private set; }
	public HashSet<string> Syms => timeMap[time].Syms;
	public IReadOnlyDictionary<string, double> Positions => positions;
	public IReadOnlyDictionary<string, bool> IsReady => isReady;
	public Serie<string> Equity => Serie.Make("Equity", prices.Index, equity);
	public IObservable<OrderUpdate> WhenOrder => whenOrder.AsObservable();



	public Broker(Frame<string, string, Bar> prices, BrokerOpts opts, Frame<string, string>[] required)
	{
		(this.prices, this.opts) = (prices, opts);
		timeMap = new TimeMap([..required, prices.Get(Bar.Open), prices.Get(Bar.High), prices.Get(Bar.Low), prices.Get(Bar.Close), prices.Get(Bar.Volume)]);
		Cash = opts.Cash;
		positions = prices.ToDictionary(e => e.Name, _ => 0.0);
		isReady = prices.ToDictionary(e => e.Name, _ => true);
		equity = new double[prices.RowCount];
	}



	public Order PlaceOrder(OrderDef def)
	{
		if (!IsReady[def.Symbol]) throw new ArgumentException("Trying to send an order for a symbol that's not ready");
		if (def.Size.IsNaN()) throw new ArgumentException("Invalid order: Size=NaN");

		var order = new Order(orderIdCounter++, time, def.Symbol, def.Dir, def.Size, def.Type);
		pending.Add(new PendingOrder(order));
		isReady[def.Symbol] = false;
		return order;
	}


	public void TickStart(int t)
	{
		time = t;
		ProcessAll();
	}

	public void TickEnd()
	{
		if (opts.UseCurrentCloseForMarketOrders)
			ProcessAll(e => e is MarketOrderType);

		ClosePositionsForSymbolsRemovedFromTheUniverse();
		
		equity[time] = Cash + positions
			.Where(kv => Syms.Contains(kv.Key))
			.Sum(kv => kv.Value * Get(kv.Key, Bar.Close).EnsureNotNaN());

		if (equity[time].IsNaN()) throw new ArgumentException("There should be no NaNs here");
	}




	void ProcessAll(Func<OrderType, bool>? filter = null)
	{
		var toDel = new HashSet<PendingOrder>();
		var toProcess = filter switch
		{
			null => pending.ToList(),
			not null => pending.Where(e => filter(e.Order.Type)).ToList(),
		};
		foreach (var pendingOrder in toProcess)
		{
			if (Process(pendingOrder))
				toDel.Add(pendingOrder);
		}
		pending.RemoveAll(toDel.Contains);
	}


	void ClosePositionsForSymbolsRemovedFromTheUniverse()
	{
		var symsToClose = timeMap[time].SymsRemoveNext.WhereA(e => Positions[e] != 0);
		foreach (var sym in symsToClose)
		{
			var dir = Positions[sym] switch
			{
				> 0 => OrderDir.Sell,
				< 0 => OrderDir.Buy,
				_ => throw new ArgumentException("Impossible"),
			};
			var size = Math.Abs(Positions[sym]);
			var orderType = new MarketOrderType(true);
			var order = new Order(orderIdCounter++, time, sym, dir, size, orderType);
			var pendingOrder = new PendingOrder(order);
			isReady[sym] = false;
			var fillTime = time + 1; // avoid having trades that open and close on the same day
			Fill(pendingOrder, Get(pendingOrder, Bar.Close), fillTime);
		}
	}


	bool Process(PendingOrder p)
	{
		if (!Syms.Contains(p.Order.Symbol))
			return true;

		switch (p)
		{
			case { Order: { Type: MarketOrderType } }:
				Fill(p, opts.UseCurrentCloseForMarketOrders ? Get(p, Bar.Close) : Get(p, Bar.Open));
				return true;
			case { Order: { Type: CloseOrderType } }:
				Fill(p, Get(p, Bar.Close));
				return true;


			case { Order: { Type: StopOrderType { Price: var price }, Dir: OrderDir.Buy } } when Get(p, Bar.High) >= price:
				Fill(p, price);
				return true;
			case { Order: { Type: StopOrderType { Price: var price }, Dir: OrderDir.Sell } } when Get(p, Bar.Low) <= price:
				Fill(p, price);
				return true;


			case { Order: { Type: LimitOrderType { Price: var price }, Dir: OrderDir.Buy } } when Get(p, Bar.Low) <= price:
				Fill(p, price);
				return true;
			case { Order: { Type: LimitOrderType { Price: var price }, Dir: OrderDir.Sell } } when Get(p, Bar.High) >= price:
				Fill(p, price);
				return true;


			case { Order: { Type: StopLimitOrderType { Price: var price }, Dir: OrderDir.Buy }, IsTriggered: false } when Get(p, Bar.High) >= price:
				p.IsTriggered = true;
				return false;
			case { Order: { Type: StopLimitOrderType { Price: var price }, Dir: OrderDir.Sell }, IsTriggered: false } when Get(p, Bar.Low) <= price:
				p.IsTriggered = true;
				return false;

			case { Order: { Type: StopLimitOrderType { PriceLimit: var priceLimit }, Dir: OrderDir.Buy }, IsTriggered: true } when Get(p, Bar.Low) <= priceLimit:
				Fill(p, priceLimit);
				return true;
			case { Order: { Type: StopLimitOrderType { PriceLimit: var priceLimit }, Dir: OrderDir.Sell }, IsTriggered: true } when Get(p, Bar.High) >= priceLimit:
				Fill(p, priceLimit);
				return true;


			default:
				return false;
		}
	}

	double Get(PendingOrder p, Bar bar) => Get(p.Order.Symbol, bar);
	double Get(string sym, Bar bar) => prices[sym, bar].Values[time];


	void Fill(PendingOrder p, double execPrice, int? timeOverride = null)
	{
		if (execPrice.IsNaN()) throw new ArgumentException("The ExecPrice shouldn't be a NaN");

		var order = p.Order;
		var priceSize = execPrice * order.Size;
		var comm = priceSize * opts.Comm;
		var cashDelta = -order.Dir.ToSign() * priceSize - comm;

		if ((Cash + cashDelta).IsNegative())
		{
			SendUpdate(p.Order, UpdateInfo.Rejected, timeOverride);
		}
		else
		{
			Cash = (Cash + cashDelta).SnapToZeroIfClose();
			positions[order.Symbol] = (positions[order.Symbol] + order.Dir.ToSign() * order.Size).SnapToZeroIfClose();
			if (positions[order.Symbol].IsNaN()) throw new ArgumentException("We shouldn't have a NaN here");

			if (Cash < 0) throw new ArgumentException("Impossible");

			SendUpdate(p.Order, UpdateInfo.Filled(execPrice, comm), timeOverride);
		}
	}


	void SendUpdate(Order order, UpdateInfo info, int? timeOverride)
	{
		if (IsReady[order.Symbol]) throw new ArgumentException("This symbol should not be ready");
		isReady[order.Symbol] = true;
		whenOrder.OnNext(new OrderUpdate(timeOverride ?? time, info, order));
	}
}
