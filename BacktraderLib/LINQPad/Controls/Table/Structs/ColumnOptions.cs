using BacktraderLib._sys.Structs;

namespace BacktraderLib;

public sealed record ColumnOptions<T>(Func<T, object> Fun, string Title, Type? ExprValueType)
{
	internal int? Width_ { get; private set; }
	public ColumnOptions<T> Width(int width) => With(e => e.Width_ = width);

	internal ColumnAlign? Align_ { get; private set; }
	public ColumnOptions<T> Align(ColumnAlign align) => With(e => e.Align_ = align);

	internal ColumnSearchInfo<T>? SearchInfo_ { get; private set; }
	public ColumnOptions<T> Searchable(Func<T, object>? funOverride = null) => With(e => e.SearchInfo_ = new ColumnSearchInfo<T>(funOverride));

	internal string? Fmt_ { get; private set; }
	/// <summary>
	/// Use formatters in ColumnFormatters
	/// </summary>
	public ColumnOptions<T> Fmt(string fmt) => With(e => e.Fmt_ = fmt);

	ColumnOptions<T> With(Action<ColumnOptions<T>> action)
	{
		action(this);
		return this;
	}
}
