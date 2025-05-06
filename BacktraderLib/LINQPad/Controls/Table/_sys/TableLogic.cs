using System.Text.Json.Nodes;
using BacktraderLib._sys.Utils;
using BaseUtils;
using LINQPad;
using RxLib;

namespace BacktraderLib._sys;

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
						.SelectMany((column, columnIdx) => DuoIf(
							TableJsonUtils.KeyVal(
								ColumnFieldName(columnIdx),
								column.Fun(item)
									.FormatEnums()
							),
							column.SearchInfo_?.FunOverride != null,
							() => TableJsonUtils.KeyVal(
								ColumnSearchFieldName(columnIdx),
								column.SearchInfo_!.FunOverride!(item)
							)
						))
						.Prepend(TableJsonUtils.KeyVal(
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
				.SelectMany((column, columnIdx) => DuoIf<object>(
					new
					{
						field = ColumnFieldName(columnIdx),
						title = column.Title,
						width = column.Width_,
						hozAlign = column.Align_.SerEnum(),
						formatter = column.Fmt_,
					},
					column.SearchInfo_?.FunOverride != null,
					() => new
					{
						field = ColumnSearchFieldName(columnIdx),
						title = ColumnSearchFieldName(columnIdx),
						visible = false,
					}
				))
				.ToJsonArray(),
		}.ToJsonObjectGen();

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
		.ToJsonObjectGen();

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
		.ToJsonObjectGen();

		// Layout
		// ------
		var jsonLayout = new
		{
			height = opts.Height,
			layout = opts.Layout.SerEnum(),
		}
		.ToJsonObjectGen();

		// => Full Config
		// --------------
		var jsonConfig = new[]
		{
			new
			{
				data = jsonDataFun(Δitems.V),
			}.ToJsonObjectGen(),
			jsonColumns,
			jsonPagination,
			jsonSelection,
			jsonLayout,
			TableInit.jsonKeybindings,
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
				.JSRepl_Obj(0, jsonConfig.SerFinal())
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



		// ********************
		// ********************
		// **** Search Bar ****
		// ********************
		// ********************
		var searchBarCtrls = new List<Tag>();
		var jsInitTextBox = "";

		if (columns.Any(e => e.SearchInfo_ != null))
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
			var jsSearchExpr = columns.Index()
				.Where(column => column.Item.SearchInfo_ != null)
				.Select(column =>
					(column.Item.SearchInfo_!.FunOverride != null) switch
					{
						false => ColumnFieldName(column.Index),
						true => ColumnSearchFieldName(column.Index),
					}
				)
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

		searchBarCtrls = [..opts.ExtraCtrlsPrepend, ..searchBarCtrls, ..opts.ExtraCtrlsAppend];



		// *******************
		// *******************
		// **** Final Tag ****
		// *******************
		// *******************
		var finalJS =
			jsInitTable +
			jsInitSelection +
			jsInitTextBox;

		if (opts.Dbg_)
			Util.SyntaxColorText(finalJS, SyntaxLanguageStyle.JavaScript).Dump();

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
					OnRenderJS = finalJS,
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
					____2____;
				})();
				""",
				e => e
					.JSRepl_Var(0, id)
					.JSRepl_Obj(1, jsonDataFun(items).Ser())
					.JSRepl_Var(2, onSelect != null ? "table.selectRow(0)" : "")
			)).D(D);



		return tag;
	}



	static string ColumnFieldName(int columnIdx) => $"c{columnIdx}";
	static string ColumnSearchFieldName(int columnIdx) => $"s{columnIdx}";


	static T[] DuoIf<T>(T first, bool condition, Func<T> second) =>
		condition switch
		{
			false => [first],
			true => [first, second()],
		};
}





