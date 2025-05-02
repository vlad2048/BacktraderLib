/*
using BacktraderLib._sys.JsonConverters;
using BacktraderLib._sys.Tabulator;
using RxLib;
using System.Linq.Expressions;

namespace BacktraderLib;

public static partial class Ctrls
{
	public static Tag ToTabulatorViewer<T>(this IRoVar<T[]> Δitems, TabulatorOptions<T> opts) => Tabulator.Make(Δitems, opts, null);
	public static Tag ToTabulatorViewer<T>(this T[] items, TabulatorOptions<T> opts) => Var.MakeConst(items).ToTabulatorViewer(opts);

	public static (IRoVar<T>, Tag) ToTabulatorSelector<T>(this IRoVar<T[]> Δitems, TabulatorOptions<T> opts)
	{
		var Δrx = Var.Make(Δitems.V[0]);
		var tag = Tabulator.Make(Δitems, opts, idx => Δrx.V = Δitems.V[idx]);
		return (Δrx, tag);
	}
	public static Tag ToTabulatorSelector<T>(this IRoVar<T[]> Δitems, IRwVar<T> Δrx, TabulatorOptions<T> opts) => Tabulator.Make(Δitems, opts, idx => Δrx.V = Δitems.V[idx]);
	public static (IRoVar<T>, Tag) ToTabulatorSelector<T>(this T[] items, TabulatorOptions<T> opts) => Var.MakeConst(items).ToTabulatorSelector(opts);
}



public sealed class TabulatorOptions<T>
{
	public int? Width { get; init; }
	public int Height { get; init; } = 300;
	public int? PageSize { get; init; }
	public TabulatorLayout? Layout { get; init; }
	public Func<List<Tag>, IEnumerable<Tag>>? ExtraControls { get; init; }

	internal List<TabulatorColumn<T>> Columns { get; } = [];
	internal List<TabulatorSearchField<T>> Searches { get; } = [];

	public TabulatorOptions<T> AddColumn(
		Expression<Func<T, object>> expr,
		string? title = null,
		TabulatorColumnFormatter formatter = TabulatorColumnFormatter.None
	)
	{
		Columns.Add(new TabulatorColumn<T>(expr, title, formatter));
		return this;
	}

	public TabulatorOptions<T> AddSearchField(
		Expression<Func<T, object>> expr
	)
	{
		Searches.Add(new TabulatorSearchField<T>(expr));
		return this;
	}
}



[PlotlyEnum(EnumStyle.CamelCase)]
public enum TabulatorLayout
{
	FitData,
	FitColumns,
	FitDataFill,
	FitDataStretch,
	FitDataTable,
}



public enum TabulatorColumnFormatter
{
	None,
	Money,
}

public sealed record TabulatorColumn<T>(
	Expression<Func<T, object>> Expr,
	string? Title,
	TabulatorColumnFormatter Formatter
);

public sealed record TabulatorSearchField<T>(
	Expression<Func<T, object>> Expr
);
*/
