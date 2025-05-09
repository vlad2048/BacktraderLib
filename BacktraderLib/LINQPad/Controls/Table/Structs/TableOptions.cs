using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using BacktraderLib._sys.Structs;
using BaseUtils;

namespace BacktraderLib;

public sealed record TableOptions<T>(
	int? Width = null,
	int? Height = 300,
	TableLayout? Layout = null,
	int? PageSize = null
)
{
	readonly List<ColumnOptions<T>> columns = [];
	internal ColumnOptions<T>[]? Columns => columns.Count > 0 ? columns.ToArray() : null;
	public TableOptions<T> Add(Func<T, object> fun, string title, Func<ColumnOptions<T>, ColumnOptions<T>>? build = null) =>
		this.With(() =>
		{
			var opt = new ColumnOptions<T>(fun, title, null);
			build?.Invoke(opt);
			columns.Add(opt);
		});
	public TableOptions<T> Add(Expression<Func<T, object>> expr, Func<ColumnOptions<T>, ColumnOptions<T>>? build = null) =>
		this.With(() =>
		{
			if (!expr.IsSimpleAccessor()) throw new ArgumentException("Expression must be a simple property accessor");
			var opt = new ColumnOptions<T>(expr.Compile(), expr.GetSimpleAccessorName(), expr.GetSimpleAccessorPropertyType());
			build?.Invoke(opt);
			columns.Add(opt);
		});

	internal Tag[] ExtraCtrlsPrepend { get; private set; } = [];
	public TableOptions<T> PrependCtrls(params Tag[] ctrls) => this.With(() => ExtraCtrlsPrepend = ExtraCtrlsPrepend.ConcatA(ctrls));

	internal Tag[] ExtraCtrlsAppend { get; private set; } = [];
	public TableOptions<T> AppendCtrls(params Tag[] ctrls) => this.With(() => ExtraCtrlsAppend = ExtraCtrlsAppend.ConcatA(ctrls));

	readonly List<SearchField<T>> searchFields = [];
	internal SearchField<T>[] SearchFields => [..searchFields];
	public TableOptions<T> Search(Func<T, object> fun, [CallerArgumentExpression(nameof(fun))] string? funName = null) => this.With(() => searchFields.Add(new SearchField<T>(fun, (funName ?? throw new ArgumentException("Impossible")).FmtSearchName())));

	internal bool Dbg_ { get; private set; }
	public TableOptions<T> Dbg() => this.With(() => Dbg_ = true);
	
	

	/*internal bool EnableCellCopy_ { get; private set; }
	public TableOptions<T> EnableCellCopy() => this.With(() => EnableCellCopy_ = true);

	internal bool DisplayRowCount_ { get; private set; }
	public TableOptions<T> DisplayRowCount() => this.With(() => DisplayRowCount_ = true);*/
}



file static class TableOptionsUtils
{
	public static string FmtSearchName(this string e) =>
		e
			.AfterArrow()
			.RemoveEDot();

	static string AfterArrow(this string e)
	{
		var xs = e.Split("=>", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
		return xs.Length switch
		{
			2 => xs[1],
			_ => e.Trim(),
		};
	}

	static string RemoveEDot(this string e) => e.Replace("e.", "");
}