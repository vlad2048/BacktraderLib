using System.Text.Json.Nodes;
using BacktraderLib._sys.FrameRendering.Utils;
using BacktraderLib._sys.Utils;
using Frames;

namespace BacktraderLib._sys.FrameRendering;

static class RendererFrame2
{
	static string ColName(int colIdx, int subColIdx) => $"c{colIdx}_{subColIdx}";

	public static Tag Render<N, K1, K2>(Frame<N, K1, K2> df)
	{
		var fmts = (
			from col in df.Index()
			from subCol in col.Item.Index()
			select new
			{
				col = col.Index,
				subCol = subCol.Index,
				fmt = SerieFmt.Make(subCol.Item.Values),
			}
		).ToDictionary(e => (e.col, e.subCol), e => e.fmt);

		var jsonData = new
			{
				data = df.Index.Select((date, dateIdx) =>
						(
							from col in df.Index()
							from subCol in col.Item.Index()
							select TableJsonUtils.KeyVal(
								ColName(col.Index, subCol.Index),
								fmts[(col.Index, subCol.Index)].Format(subCol.Item.Values[dateIdx])
							)
						)
						.Prepend(TableJsonUtils.KeyVal("date", $"{date:yyyy-MM-dd}"))
						.Prepend(TableJsonUtils.KeyVal("id", dateIdx))
						.ToJsonObject()
					)
					.ToJsonArray(),
			}
			.ToJsonObjectGen();

		JsonArray jsonColumnsFun(Func<string, string, bool>? filter) =>
			df.Select((col, colIdx) => new
				{
					title = $"{col.Name}",
					columns = col.Select((subCol, subColIdx) => new
						{
							field = ColName(colIdx, subColIdx),
							title = $"{subCol.Name}",
							hozAlign = ColumnAlign.Right.SerEnum(),
							headerSort = false,
							resizable = false,
							width = RendererConsts.ColumnWidth,
							visible = filter == null || filter($"{col.Name}", $"{subCol.Name}"),
					})
						.ToJsonArray(),
				})
				.ToJsonArray();

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
			new
			{
				columns = jsonColumnsFun(null),
			}.ToJsonObjectGen(),
			jsonLayout,
		}.Merge();


		var id = IdGen.Make();
		var idSearch = $"{id}-search";

		var jsInit = JS.Fmt(
			"""
			new Tabulator(
				elt,
				____0____
			);
			
			document.getElementById(____1____).addEventListener('input', evt => { window.dispatch(____1____, {}); });
			""",
			e => e
				.JSRepl_Obj(0, jsonConfig.SerFinal())
				.JSRepl_Val(1, idSearch)
		);


		Events.ListenFast(idSearch, () =>
		{
			var str = JS.Return("document.getElementById(____0____).value;", e => e.JSRepl_Val(0, idSearch));
			var parts = str.ToLowerInvariant().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

			bool Filter(string e, string f) => parts.All(part => e.ToLowerInvariant().Contains(part, StringComparison.OrdinalIgnoreCase));

			JS.Run(
				"""
				const table = Tabulator.findTable('#____0____')[0];
				table.setColumns(____1____);
				""",
				e => e
					.JSRepl_Var(0, id)
					.JSRepl_Obj(1, jsonColumnsFun(Filter).SerFinal())
			);
		});


		return
			new Tag("div")
			{
				Class = CtrlsClasses.TableWrapper,
				Style = FrameRenderingUtils.MakeRootStyle(RendererConsts.Frame2.Width),
				Kids =
				[
					new Tag("div")
					{
						Class = $"{CtrlsClasses.HorzCtrlRow} {CtrlsClasses.TableControls}",
						Kids =
						[
							new Tag("input", idSearch)
							{
								Attributes =
								{
									{ "type", "text" },
									{ "placeholder", "Search" },
								},
								Style =
								[
									"width: 100%",
								],
							},
						],
					},

					new Tag("div", id)
					{
						OnRenderJS = jsInit,
					},
				]
			};
	}
}
