using System.Linq.Expressions;
using BaseUtils;

namespace BacktraderLib;

public sealed record TableOptions<T>(
	int? Width = null,
	int? Height = 300,
	TableLayout? Layout = null,
	int? PageSize = null
)
{
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

	public TableOptions<T> PrependCtrls(params Tag[] ctrls)
	{
		ExtraCtrlsPrepend = ExtraCtrlsPrepend.ConcatA(ctrls);
		return this;
	}

	public TableOptions<T> AppendCtrls(params Tag[] ctrls)
	{
		ExtraCtrlsAppend = ExtraCtrlsAppend.ConcatA(ctrls);
		return this;
	}

	public TableOptions<T> Dbg()
	{
		Dbg_ = true;
		return this;
	}



	readonly List<ColumnOptions<T>> columns = [];



	internal ColumnOptions<T>[]? Columns => columns.Count > 0 ? columns.ToArray() : null;
	internal Tag[] ExtraCtrlsPrepend { get; private set; } = [];
	internal Tag[] ExtraCtrlsAppend { get; private set; } = [];
	internal bool Dbg_ { get; private set; }
}
