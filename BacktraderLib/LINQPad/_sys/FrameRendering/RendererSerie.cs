using BacktraderLib._sys.FrameRendering.Utils;
using BacktraderLib._sys.Utils;
using Frames;
using System.Text.Json.Nodes;

namespace BacktraderLib._sys.FrameRendering;

static class RendererSerie
{
	const string ColName = "c";

	public static Tag Render<N>(Serie<N> df)
	{
		var fmt = SerieFmt.Make(df.Values);

		var jsonData = new
		{
			data = df.Index.Select((date, dateIdx) =>
					new[]
						{
							TableJsonUtils.KeyVal("date", $"{date:yyyy-MM-dd}"),
							TableJsonUtils.KeyVal("id", dateIdx),
							TableJsonUtils.KeyVal(ColName, fmt.Format(df.Values[dateIdx])),
						}
						.ToJsonObject()
				)
				.ToJsonArray(),
		}.ToJsonObjectGen();

		var jsonColumns = new
			{
				columns = new object[]
					{
						new
						{
							field = ColName,
							title = $"{df.Name}",
							hozAlign = ColumnAlign.Right.SerEnum(),
							headerSort = false,
							resizable = false,
							width = RendererConsts.ColumnWidth,
						}
					}
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
			Style = FrameRenderingUtils.MakeRootStyle(RendererConsts.Serie.Width),
		};
	}
}