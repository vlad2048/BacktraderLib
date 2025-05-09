using BaseUtils;

namespace BacktraderLib;

public sealed record ColumnOptions<T>(Func<T, object> Fun, string Title, Type? ExprValueType)
{
	internal int? Width_ { get; private set; }
	public ColumnOptions<T> Width(int width) => this.With(() => Width_ = width);

	internal bool Hide_ { get; private set; }
	public ColumnOptions<T> Hide() => this.With(() => Hide_ = true);

	internal ColumnAlign? Align_ { get; private set; }
	public ColumnOptions<T> Align(ColumnAlign align) => this.With(() => Align_ = align);

	internal string? Fmt_ { get; private set; }
	/// <summary>
	/// Use formatters in ColumnFormatters
	/// </summary>
	public ColumnOptions<T> Fmt(string fmt) => this.With(() => Fmt_ = fmt);
}
