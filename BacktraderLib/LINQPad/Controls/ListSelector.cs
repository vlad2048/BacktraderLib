using System.Linq.Expressions;
using BacktraderLib._sys.ListSelector;

namespace BacktraderLib;

public static partial class Ctrls
{
	public static (IRoVar<S>, Tag) ToListSelector<T, S, U>(
		this IRoVar<T[]> source,
		Func<T, S> selFun,
		Func<T, U> dispFun,
		int? pageSize = null,
		Expression<Func<T, object>>[]? orderFuns = null,
		Func<T, string>? searchFun = null
	) => ListSelector.Make(
		source,
		selFun,
		dispFun,
		pageSize,
		orderFuns,
		searchFun
	);
}