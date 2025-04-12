using System.Linq.Expressions;
using BacktraderLib._sys.ListSelector;
using RxLib;

namespace BacktraderLib;

public static partial class Ctrls
{
	public static Tag ToListViewer<T, U>(
		this IRoVar<T[]> source,
		Func<T, U> dispFun,
		int pageSize,
		Expression<Func<T, object>>[]? orderFuns = null,
		Func<T, string>? searchFun = null
	) where T : class => ListSelector.Make<T, int, U>(
		source,
		null,
		dispFun,
		pageSize,
		orderFuns,
		searchFun
	).Item2;

	public static Tag ToListViewer<T, U>(
		this T[] source,
		Func<T, U> dispFun,
		int pageSize,
		Expression<Func<T, object>>[]? orderFuns = null,
		Func<T, string>? searchFun = null
	) where T : class =>
		Var.MakeConst(source).ToListViewer(
			dispFun,
			pageSize,
			orderFuns,
			searchFun
		);



	public static (IRoVar<S>, Tag) ToListSelector<T, S, U>(
		this IRoVar<T[]> source,
		Func<T, S> selFun,
		Func<T, U> dispFun,
		int? pageSize = null,
		Expression<Func<T, object>>[]? orderFuns = null,
		Func<T, string>? searchFun = null
	) where T : class => ListSelector.Make(
		source,
		selFun,
		dispFun,
		pageSize,
		orderFuns,
		searchFun
	);
	
	public static (IRoVar<S>, Tag) ToListSelector<T, S, U>(
		this T[] source,
		Func<T, S> selFun,
		Func<T, U> dispFun,
		int pageSize,
		Expression<Func<T, object>>[]? orderFuns = null,
		Func<T, string>? searchFun = null
	) where T : class =>
		Var.MakeConst(source).ToListSelector(
			selFun,
			dispFun,
			pageSize,
			orderFuns,
			searchFun
		);
}