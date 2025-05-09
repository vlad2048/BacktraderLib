using BacktraderLib._sys;
using RxLib;

namespace BacktraderLib;

public static partial class Ctrls
{
	public static Tag ToTable<T>(this IRoVar<T[]> Δitems, TableOptions<T>? opts = null) => TableLogic.Make(Δitems, opts, null);
	public static Tag ToTable<T>(this IEnumerable<T> items, TableOptions<T>? opts = null) => Var.MakeConst(items.ToArray()).ToTable(opts);

	public static (IRoVar<T>, Tag) ToTableSelector<T>(this IRoVar<T[]> Δitems, TableOptions<T> opts)
	{
		var Δrx = Var.Make(Δitems.V[0]);
		var tag = TableLogic.Make(Δitems, opts, idx => Δrx.V = Δitems.V[idx]);
		return (Δrx, tag);
	}
	public static Tag ToTableSelector<T>(this IRoVar<T[]> Δitems, IRwVar<T> Δrx, TableOptions<T> opts) => TableLogic.Make(Δitems, opts, idx => Δrx.V = Δitems.V[idx]);
	public static (IRoVar<T>, Tag) ToTableSelector<T>(this IEnumerable<T> items, TableOptions<T> opts) => Var.MakeConst(items.ToArray()).ToTableSelector(opts);
}
