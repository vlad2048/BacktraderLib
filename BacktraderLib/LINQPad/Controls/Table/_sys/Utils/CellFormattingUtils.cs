namespace BacktraderLib._sys.Utils;

static class CellFormattingUtils
{
	public static object? FormatEnums(this object obj) =>
		obj switch
		{
			null => null,
			_ => obj.GetType().IsEnum switch
			{
				true => $"{obj}",
				_ => obj,
			},
		};
}