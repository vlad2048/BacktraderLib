using BacktraderLib._sys;

namespace BacktraderLib;

public sealed record OrderDef(
	string Symbol,
	OrderDir Dir,
	double Size,
	OrderType Type
)
{
	public object ToDump() => new
	{
		Symbol,
		Dir,
		Size = Size.FmtDumpVal(),
		Type = $"{Type}",
	};

	public override string ToString() => $"Def[{Dir}  {Symbol}  qty:{Size:F2}  {Type}]";
}
