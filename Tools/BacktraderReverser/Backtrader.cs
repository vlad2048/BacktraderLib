using System.Text;
using BacktraderReverser.Structs;
using BacktraderReverser.Utils;

namespace BacktraderReverser;

public sealed class BrokerOpts
{
	public double Cash { get; init; } = 100;
	public double Commission { get; init; } = 0;
	public bool CheatOnClose { get; init; } = false;
}

public static class Backtrader
{
	const string PythonCodeResourceName = "BacktraderReverser.Backtrader.py";

	public static Results Run(
		Prices prices,
		Order[] orders,
		BrokerOpts brokerOpts
	)
	{
		var code = ResourceLoader.Load(PythonCodeResourceName)
			.Replace("__prices__", prices.Fmt())
			.Replace("__startDate__", $"{Consts.StartDate:yyyy-MM-dd}")
			.Replace("__pricesLength__", $"{prices.Length}")
			.Replace("__orders__", orders.Fmt())
			.Replace("__brokerCash__", $"{brokerOpts.Cash}")
			.Replace("__brokerComm__", $"{brokerOpts.Commission}")
			.Replace("__cheatOnClose__", $"{brokerOpts.CheatOnClose}")
			;
		return Python.Run(code).ReadResults();
	}


	static Results ReadResults(this string[] lines)
	{
		var (linesM, linesO, linesT) = IsolateLines(lines);
		return new Results(
			linesM.Parse(Market.Parse),
			linesO.Parse(OrderUpdate.Parse),
			linesT.Parse(Trade.Parse)
		);
	}
	static T[] Parse<T>(this string[] lines, Func<string[], T> fun) =>
		lines
			.Select(line => fun(line.Split(new[] { ',' }, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)))
			.ToArray();
	static (string[], string[], string[]) IsolateLines(string[] lines)
	{
		var idxM = lines.Index().First(e => e.Item == "# Main").Index;
		var idxO = lines.Index().First(e => e.Item == "# Orders").Index;
		var idxT = lines.Index().First(e => e.Item == "# Trades").Index;
		return (
			lines.Skip(idxM + 2).Take(idxO - idxM - 2).Clean(),
			lines.Skip(idxO + 2).Take(idxT - idxO - 2).Clean(),
			lines.Skip(idxT + 2).Clean()
		);
	}
	static string[] Clean(this IEnumerable<string> source) => source.Where(e => e != string.Empty).ToArray();



	static string Fmt(this Prices prices)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"        'Open'  : {prices.Open.Fmt()},");
		sb.AppendLine($"        'Close' : {prices.Close.Fmt()},");
		if (prices.High != null)
			sb.AppendLine($"        'High'  : {prices.High.Fmt()},");
		if (prices.Low != null)
			sb.AppendLine($"        'Low'   : {prices.Low.Fmt()},");
		return sb.ToString();
	}

	static string Fmt(this Order[] orders)
	{
		var sb = new StringBuilder();
		foreach (var order in orders)
		{
			var orderDate = Consts.StartDate + TimeSpan.FromDays(order.Date);
			sb.Append($"        if self.data.datetime.date() == dt.date({orderDate.Year}, {orderDate.Month}, {orderDate.Day}): ");
			sb.Append("self.");
			sb.Append(order.Dir == OrderDir.Buy ? "buy" : "sell");
			sb.Append($"(size={order.Size}, exectype=bt.Order.{order.Type}");
			if (order.Type is OrderType.Limit or OrderType.Stop or OrderType.StopLimit)
				sb.Append($", price={order.Price}");
			if (order.Type == OrderType.StopLimit)
				sb.Append($", plimit={order.PriceLimit}");
			sb.AppendLine(")");
		}
		return sb.ToString();
	}

	static string Fmt(this double[] arr) => $"[{string.Join(", ", arr.Select(e => $"{e}"))}]";
}