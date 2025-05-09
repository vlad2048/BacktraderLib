namespace Feed.Symbology;

public sealed record Trading212SymbolData(
	Trading212Symbol[] Symbols,
	IReadOnlyDictionary<int, Trading212ScheduleEvent[]> Schedules
);

public sealed record Trading212Symbol(
	string Ticker,
	string Type,
	string Exchange,
	int WorkingScheduleId,
	string Isin,
	string Ccy,
	string Name,
	string ShortName,
	decimal MinTradeQuantity,
	decimal MaxOpenQuantity,
	DateTime AddedOn
);


public enum Trading212ScheduleEventType
{
	Open,
	Close,
	PreMarketOpen,
	AfterHoursOpen,
	OvernightOpen,
	AfterHoursClose,
}

public sealed record Trading212ScheduleEvent(
	DateTime Date,
	Trading212ScheduleEventType Type
);
