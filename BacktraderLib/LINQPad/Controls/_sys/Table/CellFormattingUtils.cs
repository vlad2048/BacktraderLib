namespace BacktraderLib._sys.Table;

static class CellFormattingUtils
{
	public static object FormatEnums(this object obj) =>
		obj.GetType().IsEnum switch
		{
			true => $"{obj}",
			_ => obj,
		};
}