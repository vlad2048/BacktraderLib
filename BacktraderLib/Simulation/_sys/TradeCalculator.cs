namespace BacktraderLib._sys;

static class TradeCalculator
{
	public static Trade[] Calc(
		IReadOnlyList<OrderUpdate> orders,
		int lastPriceIdx,
		IReadOnlyDictionary<string, double> lastPrices
	) =>
		orders
			.FilterFills()
			.GroupBy(e => e.Order.Symbol)
			.SelectMany(grp => CalcSym(grp, grp.Key, lastPriceIdx, lastPrices[grp.Key]))
			.OrderBy(e => e.EntrDate)
			.ToArray();



	static Trade[] CalcSym(
		IEnumerable<Fill> orders,
		string sym,
		int lastPriceIdx,
		double lastPrice
	)
	{
		var trades = new List<Trade>();

		var parentId = -1;

		var inPosition = false;

		var entryIdx = 0;
		var direction = default(TradeDir);
		var entrySizeSum = 0.0;
		var entryGrossSum = 0.0;
		var entryFeesSum = 0.0;


		void EnterPosition(Fill order)
		{
			if (inPosition) throw new ArgumentException("Impossible");
			inPosition = true;
			entryIdx = order.Date;
			direction = order.Order.Dir.ToTradeDir();
			parentId++;
			entrySizeSum = 0;
			entryGrossSum = 0;
			entryFeesSum = 0;
		}

		void ExitPosition()
		{
			if (!inPosition) throw new ArgumentException("Impossible");
			inPosition = false;
			entryIdx = 0;
			direction = default;
		}


		foreach (var order in orders)
		{
			// Trade opened
			// ============
			if (!inPosition)
			{
				EnterPosition(order);
			}

			switch (direction, order.Order.Dir)
			{
				case (TradeDir.Long, OrderDir.Buy):
				case (TradeDir.Shrt, OrderDir.Sell):
					// Position increased
					// ==================
					entrySizeSum += order.Order.Size;
					entryGrossSum += order.Order.Size * order.ExecPrice;
					entryFeesSum += order.ExecComm;
					break;

				case (TradeDir.Long, OrderDir.Sell):
				case (TradeDir.Shrt, OrderDir.Buy):

					if (order.Order.Size.IsCloseOrLess(entrySizeSum))
					{
						// Trade closed
						// ============
						var exitSize = order.Order.Size.IsClose(entrySizeSum) switch
						{
							true => entrySizeSum,
							false => order.Order.Size,
						};

						// @formatter:off
						trades.Add(new Trade(
							Id:			trades.Count,
							DadId:		parentId,
							Symbol:		sym,
							Dir:		direction,
							Size:		exitSize,
							IsOpen:		false,

							EntrDate:	entryIdx,
							EntrPrice:	entryGrossSum / entrySizeSum,				// Take a size-weighted average of entry price
							EntrComm:	exitSize / entrySizeSum * entryFeesSum,		// Take a fraction of entry fees

							ExitDate:	order.Date,
							ExitPrice:	order.ExecPrice,
							ExitComm:	order.ExecComm
						));
						// @formatter:on

						if (order.Order.Size.IsClose(entrySizeSum))
						{
							// Position closed
							// ===============
							ExitPosition();
						}
						else
						{
							// Position decreased, previous orders have now less impact
							// ========================================================
							var sizeFraction = (entrySizeSum - order.Order.Size) / entrySizeSum;
							entrySizeSum *= sizeFraction;
							entryGrossSum *= sizeFraction;
							entryFeesSum *= sizeFraction;
						}
					}
					else
					{
						// Trade reversed
						// ==============

						// Close current trade
						// -------------------
						var clExitSize = entrySizeSum;
						var clExitFees = clExitSize / order.Order.Size * order.ExecComm;

						// @formatter:off
						trades.Add(new Trade(
							Id:			trades.Count,
							DadId:		parentId,
							Symbol:		sym,
							Dir:		direction,
							Size:		clExitSize,
							IsOpen:		false,

							EntrDate:	entryIdx,
							EntrPrice:	entryGrossSum / entrySizeSum,				// Take a size-weighted average of entry price
							EntrComm:	clExitSize / entrySizeSum * entryFeesSum,	// Take a fraction of entry fees

							ExitDate:	order.Date,
							ExitPrice:	order.ExecPrice,
							ExitComm:	clExitFees
						));
						// @formatter:on

						// Open a new trade
						// ----------------
						entrySizeSum = order.Order.Size - clExitSize;
						entryGrossSum = entrySizeSum * order.ExecPrice;
						entryFeesSum = order.ExecComm - clExitFees;
						entryIdx = order.Date;
						direction = direction.Neg();
						parentId++;
					}
					break;
			}
		}


		if (inPosition)
		{
			if ((-entrySizeSum).IsLess(0))
			{
				// Trade hasn't been closed
				// ========================
				var exitSize = entrySizeSum;

				// @formatter:off
				trades.Add(new Trade(
					Id:			trades.Count,
					DadId:		parentId,
					Symbol:		sym,
					Dir:		direction,
					Size:		exitSize,
					IsOpen:		true,

					EntrDate:	entryIdx,
					EntrPrice:	entryGrossSum / entrySizeSum,				// Take a size-weighted average of entry price
					EntrComm:	exitSize / entrySizeSum * entryFeesSum,		// Take a fraction of entry fees

					ExitDate:	lastPriceIdx,
					ExitPrice:	lastPrice,
					ExitComm:	0.0
				));
				// @formatter:on
			}
		}


		return [.. trades];
	}
}




file static class TradeCalculatorUtils
{
	public static TradeDir ToTradeDir(this OrderDir dir) => dir switch
	{
		OrderDir.Buy => TradeDir.Long,
		OrderDir.Sell => TradeDir.Shrt,
		_ => throw new ArgumentException(),
	};

	public static TradeDir Neg(this TradeDir dir) => dir switch
	{
		TradeDir.Long => TradeDir.Shrt,
		TradeDir.Shrt => TradeDir.Long,
		_ => throw new ArgumentException(),
	};



	const double RelTol = 1e-9;
	const double AbsTol = 1e-12;


	public static bool IsCloseOrLess(this double a, double b) =>
		a.IsClose(b) switch
		{
			true => true,
			false => a < b,
		};

	public static bool IsLess(this double a, double b) =>
		a.IsClose(b) switch
		{
			true => false,
			false => a < b,
		};

	public static bool IsClose(this double a, double b) =>
		(a.IsNaNOrInf() || b.IsNaNOrInf()) switch
		{
			// ReSharper disable once CompareOfFloatsByEqualityOperator
			false => (a == b) switch
			{
				true => true,
				false => Math.Abs(a - b) <= Math.Max(RelTol * Math.Max(Math.Abs(a), Math.Abs(b)), AbsTol),
			},
			true => false,
		};

	static bool IsNaNOrInf(this double v) => v.IsNaN() || v.IsInf();

	static bool IsInf(this double v) => double.IsInfinity(v);
}