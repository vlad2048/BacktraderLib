using BacktraderLib._sys.Utils;
using Frames;
using BacktraderLib._sys.FrameRendering.Utils;

namespace BacktraderLib._sys.FrameRendering;

static class RendererFrame
{
	static string ColName(int colIdx) => $"c{colIdx}";

	public static Tag Render<N, K1>(Frame<N, K1> df)
	{
		var fmts = df.ToDictionary(e => e, e => SerieFmt.Make(e.Values));

		var jsonData = new
			{
				data = df.Index.Select((date, dateIdx) =>
						df
							.Select((col, colIdx) => TableJsonUtils.KeyVal(
								ColName(colIdx),
								fmts[col].Format(col.Values[dateIdx]))
							)
							.Prepend(TableJsonUtils.KeyVal("date", $"{date:yyyy-MM-dd}"))
							.Prepend(TableJsonUtils.KeyVal("id", dateIdx))
							.ToJsonObject()
					)
					.ToJsonArray(),
			}
			.ToJsonObjectGen();

		var jsonColumns = new
			{
				columns = df.Select((col, colIdx) => new
					{
						field = ColName(colIdx),
						title = $"{col.Name}",
						hozAlign = ColumnAlign.Right.SerEnum(),
						headerSort = false,
						//resizable = false,
						//width = RendererConsts.ColumnWidth,
					})
					.ToJsonArray(),
			}
			.ToJsonObjectGen();

		var jsonLayout = new
			{
				height = RendererConsts.Height,
				layout = TableLayout.FitColumns.SerEnum(),
				rowHeader = new
				{
					field = "date",
					rowHandle = false,
					headerSort = false,
					width = RendererConsts.IndexWidth,
					resizable = false,
				},
			}
			.ToJsonObjectGen();

		var jsonConfig = new[]
		{
			jsonData,
			jsonColumns,
			jsonLayout,
		}.Merge();

		var jsInit = JS.Fmt(
			"""
			new Tabulator(
				elt,
				____0____
			);
			""",
			e => e
				.JSRepl_Obj(0, jsonConfig.SerFinal())
		);

		return new Tag("div")
		{
			OnRenderJS = jsInit,
			Style = FrameRenderingUtils.MakeRootStyle(RendererConsts.Frame.Width),
		};
	}
}