namespace Feed.Yahoo._sys.Errors;

/*
	Random errors (TODO: detect and retry them)
	=============

		YahooException: HttpResponse
		============================
			Symbol: SUH.L
			Messages: HttpStatusCode:InternalServerError YahooError:YahooError { Code = Internal Server Error, Description = Error occurred while retrieving timeseries from Redis, keys: [RedisKey [key=SUH.L, cluster=finance]] }
			ErrorFilename: _


		YahooException: HttpRequest
		===========================
			Symbol: TH.TO
			Messages: [HttpRequestException] Error while copying content to a stream.
			[IOException] Unable to read data from the transport connection: An established connection was aborted by the software in your host machine..
			[SocketException] An established connection was aborted by the software in your host machine.
			ErrorFilename: _
*/
static class YahooErrorClassifier
{
	internal const string Msg_ResultMetaFirstTradeDate_is_null = "Result.Meta.FirstTradeDate == null";
	internal const string Msg_ResultTimestamp_is_null = "Result.Timestamp == null";

	// @formatter:off
	static readonly HashSet<string> ignoreSymbols =
	[
		"ATST.L",		// Response.Chart.Error != null (YahooError { Code = internal-error, Description = Duplicate key 1521014400 })
		"HFEL.L",		// Response.Chart.Error != null (YahooError { Code = internal-error, Description = Duplicate key 1525244400 })
		"SJG.L",		// Response.Chart.Error != null (YahooError { Code = internal-error, Description = Duplicate key 1507705200 })
	];
	// @formatter:on

	public static bool IsIgnoredSymbol(string symbol) =>
		ignoreSymbols.Contains(symbol) ||
		symbol.EndsWith(".IDX");


	public static bool DoesErrorNeedResponseFileWritten(this YahooException ex) =>
		!ex.Nfo.IsErrorAcceptable() &&
		ex is
		{
			ErrorFilename: not null,
			ResponseString: not null,
		};


	static bool IsErrorAcceptable(this YahooExceptionNfo nfo) => nfo.Type switch
	{
		YahooExceptionType.HttpResponse => nfo.Messages.Length switch
		{
			1 => nfo.Messages[0] switch
			{
				"HttpStatusCode:NotFound YahooError:YahooError { Code = Not Found, Description = No data found, symbol may be delisted }" => true,
				_ => false,
			},
			_ => false,
		},
		YahooExceptionType.InvalidResponse => nfo.Messages.Length switch
		{
			1 => nfo.Messages[0] switch
			{
				Msg_ResultMetaFirstTradeDate_is_null => true,
				Msg_ResultTimestamp_is_null => true,
				_ => false,
			},
			_ => false,
		},
		_ => false,
	};
}