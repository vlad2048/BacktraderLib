using System.Linq.Expressions;
using BacktraderLib._sys.Table;
using BaseUtils;
using RxLib;

namespace BacktraderLib;

public static partial class Ctrls
{
	public static Tag ToTableViewer<T>(this IRoVar<T[]> Δitems, TableOptions<T> opts) => TableLogic.Make(Δitems, opts, null);
	public static Tag ToTableViewer<T>(this IEnumerable<T> items, TableOptions<T> opts) => Var.MakeConst(items.ToArray()).ToTableViewer(opts);

	public static (IRoVar<T>, Tag) ToTableSelector<T>(this IRoVar<T[]> Δitems, TableOptions<T> opts)
	{
		var Δrx = Var.Make(Δitems.V[0]);
		var tag = TableLogic.Make(Δitems, opts, idx => Δrx.V = Δitems.V[idx]);
		return (Δrx, tag);
	}
	public static Tag ToTableSelector<T>(this IRoVar<T[]> Δitems, IRwVar<T> Δrx, TableOptions<T> opts) => TableLogic.Make(Δitems, opts, idx => Δrx.V = Δitems.V[idx]);
	public static (IRoVar<T>, Tag) ToTableSelector<T>(this IEnumerable<T> items, TableOptions<T> opts) => Var.MakeConst(items.ToArray()).ToTableSelector(opts);
}



public sealed record TableOptions<T>(
	int? Width = null,
	int Height = 300,
	TableLayout? Layout = null,
	int? PageSize = null
)
{
	readonly List<ColumnOptions<T>> columns = [];

	internal ColumnOptions<T>[]? Columns => columns.Count > 0 ? columns.ToArray() : null;

	public TableOptions<T> Add(Func<T, object> fun, string title, Func<ColumnOptions<T>, ColumnOptions<T>>? build = null)
	{
		var opt = new ColumnOptions<T>(fun, title, null);
		build?.Invoke(opt);
		columns.Add(opt);
		return this;
	}

	public TableOptions<T> Add(Expression<Func<T, object>> expr, Func<ColumnOptions<T>, ColumnOptions<T>>? build = null)
	{
		if (!expr.IsSimpleAccessor()) throw new ArgumentException("Expression must be a simple property accessor");
		var opt = new ColumnOptions<T>(expr.Compile(), expr.GetSimpleAccessorName(), expr.GetSimpleAccessorPropertyType());
		build?.Invoke(opt);
		columns.Add(opt);
		return this;
	}
}


public sealed record ColumnOptions<T>(Func<T, object> Fun, string Title, Type? ExprValueType)
{
	internal int? Width_ { get; private set; }
	public ColumnOptions<T> Width(int width) => With(e => e.Width_ = width);

	internal ColumnAlign? Align_ { get; private set; }
	public ColumnOptions<T> Align(ColumnAlign align) => With(e => e.Align_ = align);

	internal SearchType? SearchType_ { get; private set; }
	public ColumnOptions<T> SearchType(SearchType searchType) => With(e => e.SearchType_ = searchType);

	internal string? Fmt_ { get; private set; }
	public ColumnOptions<T> Fmt(string fmt) => With(e => e.Fmt_ = fmt);

	ColumnOptions<T> With(Action<ColumnOptions<T>> action)
	{
		action(this);
		return this;
	}
}
