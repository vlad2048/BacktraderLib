using BacktraderLib._sys;
using BaseUtils;

namespace BacktraderLib;

public abstract record UpdateInfo
{
	public static FilledUpdateInfo Filled(double ExecPrice, double ExecComm) => new(ExecPrice, ExecComm);
	public static readonly RejectedUpdateInfo Rejected = new();
}

// @formatter:off
public sealed record FilledUpdateInfo(double ExecPrice, double ExecComm) : UpdateInfo { public override string ToString() => $"Filled(price:{ExecPrice.FmtDumpVal()}, comm:{ExecComm.FmtDumpVal()})"; }
public sealed record RejectedUpdateInfo : UpdateInfo { public override string ToString() => "Rejected"; }
// @formatter:on


public sealed record OrderUpdate(
	int Date,
	UpdateInfo Info,
	Order Order
)
{
	public object ToDump() => new
	{
		Date,
		Info = $"{Info}",
		Order.Symbol,
		Order.Dir,
		Size = Order.Size.FmtDumpVal(),
		Type = $"{Order.Type}",
	};
	public override string ToString() => $"Update[date:{Date}  {Info}  for  {Order}]";
}






public sealed record Fill(int Date, Order Order, double ExecPrice, double ExecComm);



public static class OrderUpdateExt
{
	public static Fill[] FilterFills(this IEnumerable<OrderUpdate> orders) =>
		orders
			.Where(e => e.Info is FilledUpdateInfo)
			.SelectA(e => new Fill(e.Date, e.Order, ((FilledUpdateInfo)e.Info).ExecPrice, ((FilledUpdateInfo)e.Info).ExecComm));
}