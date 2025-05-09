using System.Text.Json.Nodes;
using BacktraderLib._sys.Utils;
using BaseUtils;
using LINQPad;
using RxLib;

namespace BacktraderLib._sys;

static class TableLogic
{
	const int DbgItemCount = 3;


	public static Tag Make<T>(
		IRoVar<T[]> Δitems,
		TableOptions<T>? opts,
		Action<int>? onSelect
	)
	{
		if (onSelect != null && Δitems.V.Length == 0) throw new ArgumentException("Empty array not supported for a TableSelector");
		opts ??= new TableOptions<T>();
		var columns = opts.Columns ?? ColumnGuesser.Guess<T>();
		var id = IdGen.Make();

		var enableCellCopy = onSelect == null;
		var displayRowCount = true;


		// *********************
		// *********************
		// **** Json Config ****
		// *********************
		// *********************

		// Data
		// ----
		JsonArray jsonDataFun(T[] items) =>
			items
				.TakeDbg(opts.Dbg_)
				.Select((item, itemIdx) =>
					columns
						.Select((column, columnIdx) => TableJsonUtils.KeyVal(
							ColumnFieldName(columnIdx),
							column.Fun(item)
								.FormatEnums()
						))
						.Concat(opts.SearchFields.Select((searchField, searchFieldIdx) => TableJsonUtils.KeyVal(
							SearchFieldName(searchFieldIdx),
							searchField.Fun(item)
								.FormatEnums()
						)))
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
				.Select((column, columnIdx) =>
					(object)new
					{
						field = ColumnFieldName(columnIdx),
						title = column.Title,
						width = column.Width_,
						hozAlign = column.Align_.SerEnum(),
						formatter = column.Fmt_,
						visible = !column.Hide_,
					}
				)
				.Concat(opts.SearchFields.Select((_, searchFieldIdx) =>
					new
					{
						field = SearchFieldName(searchFieldIdx),
						title = SearchFieldName(searchFieldIdx),
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
		var jsonSelection = (
				(onSelect, enableCellCopy) switch
				{
					(_, true) => (object)new
					{
						selectableRange = 1,
						selectableRangeColumns = true,
						selectableRangeRows = true,
						selectableRangeClearCells = true,
						clipboard = true,
						clipboardCopyStyled = false,
						clipboardCopyConfig = new
						{
							rowHeaders = false,
							columnHeaders = false,
						},
						clipboardCopyRowRange = "range",
						clipboardPasteParser = "range",
						clipboardPasteAction = "range",
					},
					(not null, false) => new
					{
						selectableRows = 1,
					},
					(null, false) => new
					{
					},
				}
		)
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
		var jsInitSearch = "";

		if (opts.SearchFields.Any())
			jsInitSearch =
				JS.Fmt(
					"""
					const elts = ____0____.map(e => document.getElementById(e));

					function search() {
						const chop = str => str.toLowerCase().split(' ').map(e => e.trim()).filter(e => e !== '');
						const xss = elts.map(elt => chop(elt.value));
						table.setFilter(
							(row, filterParams) => {
								const { xss } = filterParams;
								let result = true;
								for (var i = 0; i < ____1____; i++) {
									const str = row[`s${i}`].toLowerCase();
									const xs = xss[i];
									result &= xs.every(part => str.includes(part));
								}
								return result;
							},
							{
								xss,
							},
						);
					}

					elts.forEach(elt => elt.addEventListener('keyup', _ => search()));
					""",
					e => e
						.JSRepl_Arr(0, opts.SearchFields.SelectA((_, searchFieldIdx) => SearchId(id, searchFieldIdx)))
						.JSRepl_Val(1, opts.SearchFields.Length)
				);

		var searchBarCtrls = opts.SearchFields
			.Select((searchField, searchFieldIdx) => new Tag("input", SearchId(id, searchFieldIdx))
			{
				Attributes =
				{
					{ "type", "text" },
					{ "placeholder", searchField.Name },
				},
				Style =
				[
					"width: 100%",
				],
			})
			.ToList();

		searchBarCtrls = [..opts.ExtraCtrlsPrepend, ..searchBarCtrls, ..opts.ExtraCtrlsAppend];

		if (displayRowCount)
			searchBarCtrls = [new Tag("span", RowCountId(id)), ..searchBarCtrls];

		var jsInitDisplayRowCount = displayRowCount switch
		{
			false => "",
			true => JS.Fmt(
				"""
				
				table.on("dataFiltered", function (filters, rows) {
					document.getElementById(____0____).innerText = `rows:${rows.length}`;
				});
				""",
				e => e
					.JSRepl_Val(0, RowCountId(id))
			),
		};


		// *******************
		// *******************
		// **** Final Tag ****
		// *******************
		// *******************
		var finalJS =
			jsInitTable +
			jsInitSelection +
			jsInitSearch +
			jsInitDisplayRowCount;

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
	static string SearchFieldName(int columnIdx) => $"s{columnIdx}";
	static string SearchId(string tableId, int columnIdx) => $"{tableId}-s{columnIdx}";
	static string RowCountId(string tableId) => $"{tableId}-rowcount";

	static T[] TakeDbg<T>(this T[] xs, bool dbg) => dbg switch
	{
		false => xs,
		true => xs.Take(DbgItemCount).ToArray(),
	};
}





