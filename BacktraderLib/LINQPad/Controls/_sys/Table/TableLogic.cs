using RxLib;
using System.Text.Json.Nodes;
using System.Text.Json;
using BacktraderLib._sys.Utils;
using LINQPad;
using System.Text.Encodings.Web;
using BaseUtils;

namespace BacktraderLib._sys.Table;

static class TableLogic
{
	public static Tag Make<T>(
		IRoVar<T[]> Δitems,
		TableOptions<T> opts,
		Action<int>? onSelect
	)
	{
		if (onSelect != null && Δitems.V.Length == 0) throw new ArgumentException("Empty array not supported for a TableSelector");
		var columns = opts.Columns ?? ColumnGuesser.Guess<T>();


		// *********************
		// *********************
		// **** Json Config ****
		// *********************
		// *********************

		// Data
		// ----
		JsonArray jsonDataFun(T[] items) =>
			items
				.Select((item, itemIdx) =>
					columns
						.Index()
						.Select(t => JsonUtils.KeyVal(
							ColumnFieldName(t.Index),
							t.Item.Fun(item)
								.FormatEnums()
						))
						.Prepend(JsonUtils.KeyVal(
							"id",
							itemIdx
						))
						.ToJsonObject()
				)
				.ToJsonArray();

		// Columns
		// -------
		var jsonColumns = new
		{
			columns = columns
				.Select((e, i) => new
				{
					field = ColumnFieldName(i),
					title = e.Title,
					hozAlign = e.Align_.SerEnum(),
				})
				.ToJsonArray(),
		}.ToJsonObject();

		// Pagination
		// ----------
		var jsonPagination = (opts.PageSize switch
		{
			not null => (object)new
			{
				pagination = true,
				paginationSize = opts.PageSize,
				paginationButtonCount = 3,
				paginationCounter = "rows",
			},
			null => new
			{
				pagination = false,
			},
		})
		.ToJsonObject();

		// Selection
		// ---------
		var jsonSelection = (onSelect switch
		{
			not null => (object)new
			{
				selectableRows = 1,
			},
			null => new
			{
			},
		})
		.ToJsonObject();

		// Layout
		// ------
		var jsonLayout = new
		{
			height = opts.Height,
			layout = opts.Layout.SerEnum(),
		}
		.ToJsonObject();

		// => Full Config
		// --------------
		var jsonConfig = new[]
		{
			new
			{
				data = jsonDataFun(Δitems.V),
			}.ToJsonObject(),
			jsonColumns,
			jsonPagination,
			jsonSelection,
			jsonLayout,
		}.Merge();



		// *****************
		// *****************
		// **** JS Init ****
		// *****************
		// *****************
		var id = IdGen.Make();

		var jsInitTable = JS.Fmt(
			"""
			const table = new Tabulator(
				elt,
				____0____
			);
			""",
			e => e
				.JSRepl_Obj(0, jsonConfig.Ser())
		);
		var jsInitSelection = onSelect switch
		{
			not null => JS.Fmt(
				"""
				
				table.on('rowSelected', _ => {
					window.dispatch(____0____, {});
				});
				table.on('tableBuilt', _ => {
					table.selectRow(0);
				});
				""",
				e => e
					.JSRepl_Val(0, id)
			),
			null => "",
		};

		//jsInit.Dump();



		// ********************
		// ********************
		// **** Search Bar ****
		// ********************
		// ********************
		var searchBarCtrls = new List<Tag>();
		var jsInitTextBox = "";

		if (columns.Any(e => e.SearchType_ is SearchType.TextBox))
		{
			var idTextBox = $"{id}-search";
			searchBarCtrls.Add(new Tag("input", idTextBox)
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
			});
			var jsSearchExpr = columns
				.Index()
				.Where(t => t.Item.SearchType_ is SearchType.TextBox)
				.Select(t => ColumnFieldName(t.Index))
				.Select(e => $$"""${row.{{e}}}""")
				.JoinText(" ");
			jsInitTextBox = JS.Fmt(
				"""
				
				document.getElementById(____0____).addEventListener('keyup', evt => {
					const parts = evt.target.value.toLowerCase().split(' ').map(e => e.trim()).filter(e => e !== '');
					table.setFilter(
						(row, filterParams) => {
							const { parts } = filterParams;
							const str = `____1____`.toLowerCase();
							const result = parts.every(part => str.includes(part));
							return result;
						},
						{
							parts,
						},
					);
				});
				""",
				e => e
					.JSRepl_Val(0, idTextBox)
					.JSRepl_Var(1, jsSearchExpr)
			);
		}

		foreach (var t in columns.Index().Where(t => t.Item.SearchType_ is SearchType.DropDown))
		{
			if (t.Item.ExprValueType?.IsEnum != true) throw new ArgumentException("For now, SearchType.DropDown only works if you use Add(Expression<Func<T, object>>) that references a property of an Enum type");

			throw new NotImplementedException("Actually SearchType.DropDown is not yet implemented at all");
		}



		// *******************
		// *******************
		// **** Final Tag ****
		// *******************
		// *******************
		var tag = new Tag("div")
		{
			Class = CtrlsClasses.TableWrapper,
			Style = opts.Width.HasValue
				?
				[
					$"width: {opts.Width}px",
					"display: inline-block",
				]
				: [],

			Kids =
			[
				..searchBarCtrls.Any()
					? (Tag[])
					[
						new Tag("div")
						{
							Class = $"{CtrlsClasses.HorzCtrlRow} {CtrlsClasses.TableControls}",
							Kids = [..searchBarCtrls],
						},
					]
					: [],

				new Tag("div", id)
				{
					OnRenderJS =
						jsInitTable +
						jsInitSelection +
						jsInitTextBox,
				},
			],
		};



		// *******************
		// *******************
		// **** Selection ****
		// *******************
		// *******************
		if (onSelect != null)
		{
			Events.ListenFast(id, () =>
			{
				var str = JS.Return(
					"""
					(function() {
						const table = Tabulator.findTable('#____0____')[0];
						const rows = table.getSelectedRows();
						if (rows.length !== 1) return -1;
						return rows[0].getIndex();
					})();
					""",
					e => e
						.JSRepl_Var(0, id)
				);
				var idx = int.Parse(str);
				if (idx == -1) return;
				onSelect(idx);
			});
		}



		// **********************
		// **********************
		// **** Update Items ****
		// **********************
		// **********************
		Δitems
			.Skip(1)
			.Subscribe(items => JS.Run(
				"""
				(async () => {
					const table = Tabulator.findTable('#____0____')[0];
					await table.setData(
						____1____
					);
					____2____
				})();
				""",
				e => e
					.JSRepl_Var(0, id)
					.JSRepl_Obj(1, jsonDataFun(items).Ser())
					.JSRepl_Var(2, onSelect != null ? "table.selectRow(0);" : "")
			)).D(D);



		return tag;
	}



	static string ColumnFieldName(int columnIdx) => $"c{columnIdx}";
}





file static class JsonUtils
{
	public static JsonArray ToJsonArray(this IEnumerable<JsonObject> items) => new(items.OfType<JsonNode?>().ToArray());
	public static JsonArray ToJsonArray<T>(this IEnumerable<T> items) => items.Ser().Deser<JsonArray>();

	public static JsonObject ToJsonObject(this IEnumerable<KeyValuePair<string, JsonNode?>> items) => new(items.ToArray());
	public static JsonObject ToJsonObject<T>(this T obj) => obj.Ser().Deser<JsonObject>();

	public static JsonObject Merge(this IEnumerable<JsonObject> objs) =>
		new(
			from obj in objs
			from kv in obj
			select new KeyValuePair<string, JsonNode>(
				kv.Key,
				kv.Value.Ser().Deser<JsonNode>()
			)
		);

	public static KeyValuePair<string, JsonNode?> KeyVal<T>(string key, T val) => new(
		key,
		JsonValue.Create(val)
	);

	public static string? SerEnum<E>(this E? val) where E : struct, Enum => val.HasValue switch
	{
		true => JsonEnumUtils.Ser(val.Value).RemoveEnclosingDoubleQuotes(),
		false => null,
	};

	public static string Ser<T>(this T obj) => JsonSerializer.Serialize(obj, jsonOpt);
	
	static T Deser<T>(this string str) => JsonSerializer.Deserialize<T>(str, jsonOpt)!;



	static string RemoveEnclosingDoubleQuotes(this string s)
	{
		if (s.Length < 2) return s;
		if (s[0] == '"' && s[^1] == '"') return s[1..^1];
		return s;
	}


	static readonly JsonSerializerOptions jsonOpt = new()
	{
		WriteIndented = true,
		IndentCharacter = '\t',
		IndentSize = 1,
		Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
	};
}
