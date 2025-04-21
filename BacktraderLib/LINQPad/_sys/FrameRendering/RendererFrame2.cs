using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using BacktraderLib._sys.Utils;
using Frames;

namespace BacktraderLib._sys.FrameRendering;


file static class Consts
{
	public const int Width = 800;
	public const int Height = 400;
	public const TabulatorLayout Layout = TabulatorLayout.FitColumns;
	public const int IndexWidth = 90;
	public const int ColumnWidth = 80;
}


static class RendererFrame2
{
	public static Tag Render<N, K1, K2>(Frame<N, K1, K2> df)
	{
		var id = IdGen.Make();
		var idSearch = $"{id}-search";

		var js = JS.Fmt(
			"""
			const table = new Tabulator(
				elt,
				{
					height: ____0____,
					layout: ____1____,
					columns: ____2____,
					data: ____3____,
					rowHeader: {
						field: 'date',
						rowHandle: false,
						headerSort: false,
						width: ____4____,
						resizable: false,
					},
				}
			);
			
			document.getElementById(____9____).addEventListener('input', evt => { window.dispatch(____9____, {}); });
			""",
			e => e
				.JSRepl_Val(0, Consts.Height)
				.JSRepl_Var(1, JsonEnumUtils.Ser(Consts.Layout))
				.JSRepl_Obj(2, JsonUtils.GetColumnDefs(df, null))
				.JSRepl_Obj(3, JsonUtils.GetData(df))
				.JSRepl_Val(4, Consts.IndexWidth)

				.JSRepl_Val(9, idSearch)
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
					.JSRepl_Obj(1, JsonUtils.GetColumnDefs(df, Filter))
			);
		});

		var tag =
			new Tag("div")
			{
				Class = CtrlsClasses.TableWrapper,
				Style =
				[
					$"width: {Consts.Width}px",
					"display: inline-block",
				],
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
						OnRenderJS = js,
					},
				],
			};

		return tag;
	}
}



file static class JsonUtils
{
	static readonly JsonSerializerOptions jsonOpt = new()
	{
		WriteIndented = true,
		IndentCharacter = '\t',
		IndentSize = 1,
		NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
	};



	public static string GetColumnDefs<N, K1, K2>(Frame<N, K1, K2> df, Func<string, string, bool>? filter) => new JsonArray(
		df
			.Select(e => new JsonObject([
				KeyVal("title", JsonValue.Create(e.Name)),
				KeyVal("columns", new JsonArray(
					e.Select(f => new JsonObject([
							KeyVal("title", JsonValue.Create($"{f.Name}")),
							KeyVal("field", JsonValue.Create($"{e.Name}_{f.Name}")),
							KeyVal("headerSort", JsonValue.Create(false)),
							KeyVal("visible", JsonValue.Create(filter == null || filter($"{e.Name}", $"{f.Name}"))),
							KeyVal("resizable", JsonValue.Create(false)),
							KeyVal("width", JsonValue.Create(Consts.ColumnWidth)),
						]))
						.Cast<JsonNode>()
						.ToArray()
				)),
			]))
			.Cast<JsonNode>()
			.ToArray()
	).Ser();



	public static string GetData<N, K1, K2>(Frame<N, K1, K2> df) => new JsonArray(
		df.Index.Index().Select(t => new JsonObject(
				(
					from e in df
					from f in e
					select KeyVal($"{e.Name}_{f.Name}", JsonValue.Create(f.Values[t.Index]))
				)
				.Prepend(KeyVal("date", JsonValue.Create($"{t.Item:yyyy-MM-dd}")))
				.Prepend(KeyVal("id", JsonValue.Create(t.Index)))
			))
			.Cast<JsonNode>()
			.ToArray()
	).Ser();



	static string Ser<T>(this T obj) => JsonSerializer.Serialize(obj, jsonOpt);

	static KeyValuePair<string, JsonNode?> KeyVal(string key, JsonNode? val) => new(key, val);
}