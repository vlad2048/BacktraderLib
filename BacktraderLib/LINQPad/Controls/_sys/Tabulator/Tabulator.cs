using BaseUtils;
using System.Linq.Expressions;
using System.Text.Json.Nodes;
using System.Text.Json;
using BacktraderLib._sys.JsonConverters;
using RxLib;

namespace BacktraderLib._sys.Tabulator;

static class Tabulator
{
	public static Tag Make<T>(
		IRoVar<T[]> Δitems,
		TabulatorOptions<T> opts,
		Action<int>? onSelect
	)
	{
		if (onSelect != null && Δitems.V.Length == 0) throw new ArgumentException("Empty array not supported");

		var id = IdGen.Make();
		var idSearch = $"{id}-search";
		var props = JsonUtils.CompileProps(opts.Columns);
		var createJS = MakeCreateJS(Δitems.V, opts, props, true);

		if (onSelect != null)
		{
			createJS += JS.Fmt(
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
			);
		}


		var tableCtrls = new List<Tag>();


		if (opts.Searches.Any())
		{
			var searchCodeJS = opts.Searches
				.Select(e => ExprUtils.GetName(e.Expr))
				.Select(e => $$"""${row.{{e}}}""")
				.JoinText(" ");
			createJS += JS.Fmt(
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
					.JSRepl_Val(0, idSearch)
					.JSRepl_Var(1, searchCodeJS)
			);
			tableCtrls.Add(
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
				}
			);
		}

		if (opts.ExtraControls != null)
		{
			tableCtrls = opts.ExtraControls(tableCtrls).ToList();
		}


		var tag =
			new Tag("div")
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
					..tableCtrls.Any()
						? (Tag[])
						[
							new Tag("div")
							{
								Class = $"{CtrlsClasses.HorzCtrlRow} {CtrlsClasses.TableControls}",
								Kids = [..tableCtrls],
							},
						]
						: [],

					new Tag("div", id)
					{
						OnRenderJS = createJS,
					},
				],
			};

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
					.JSRepl_Obj(1, items.SerWithProps(props))
					.JSRepl_Var(2, onSelect != null ? "table.selectRow(0);" : "")
			)).D(D);

		return tag;
	}



	static string MakeCreateJS<T>(
		T[] items,
		TabulatorOptions<T> opts,
		Prop<T>[] props,
		bool enableSelection
	)
	{
		var columnsCfg = opts.Columns.Zip(props)
			.Select(t => new
			{
				title = t.First.Title ?? t.Second.Name,
				field = t.Second.Name,
				formatter = $"TabulatorColumnFormatter.{t.First.Formatter}",
			})
			.Ser();

		
		var paginationCfg = opts.PageSize.HasValue switch
		{
			true => JS.Fmt(
				"""
				{
					pagination: true,
					paginationSize: ____0____,
					paginationButtonCount: 2,
					paginationCounter: 'rows',
				}
				""",
				e => e
					.JSRepl_Val(0, opts.PageSize.Value)
			),
			false =>
				"""
				{
					pagination: false,
				}
				""",
		};


		var selectionCfg = enableSelection switch
		{
			false => "{}",
			true =>
				"""
				{
					selectableRows: 1,
				}
				""",
		};



		var data = items.SerWithProps(props);

		return JS.Fmt(
			"""
			const table = new Tabulator(
				elt,
				{
					height: ____1____,
					layout: ____2____,
					
					columns: ____3____,
					
					...____4____,
					
					...____5____,
					
					data: ____6____,
				}
			);
			""",
			e => e
				.JSRepl_Val(1, opts.Height)
				.JSRepl_Var(2, opts.Layout.HasValue ? JsonEnumUtils.Ser(opts.Layout.Value) : "undefined")
				.JSRepl_Obj(3, columnsCfg)
				.JSRepl_Obj(4, paginationCfg)
				.JSRepl_Obj(5, selectionCfg)
				.JSRepl_Obj(6, data)
				.Replace("\"TabulatorColumnFormatter.None\"", "undefined")
				.Replace("\"TabulatorColumnFormatter.Money\"", "function(cell){ return tabulator_formatMoney(cell.getValue()); }")
		);
	}



	static string Check(this string s)
	{
		Console.WriteLine(s);
		return s;
	}
}



sealed record Prop<T>(string Name, Func<T, object> Get);

file static class JsonUtils
{
	public static Prop<T>[] CompileProps<T>(IEnumerable<TabulatorColumn<T>> cols) =>
		cols
			.SelectA(e =>
			{
				var name = e.Title ?? ExprUtils.GetName(e.Expr);
				var get = ExprUtils.GetGetter(e.Expr);
				if (name == "id")
					throw new ArgumentException("Column name 'id' is reserved");
				return new Prop<T>(name, get);
			});

	public static string SerWithProps<T>(this IEnumerable<T> arr, IEnumerable<Prop<T>> props) => arr.ToJsonArray(props).Ser();

	public static string Ser<T>(this T obj) => JsonSerializer.Serialize(obj, jsonOpt);


	static readonly JsonSerializerOptions jsonOpt = new()
	{
		WriteIndented = true,
		IndentCharacter = '\t',
		IndentSize = 1,
	};

	static JsonArray ToJsonArray<T>(this IEnumerable<T> arr, IEnumerable<Prop<T>> props) =>
		new(
			arr.SelectA((e, i) => (JsonNode?)e.ToJsonObject(props, i))
		);

	static JsonObject ToJsonObject<T>(this T obj, IEnumerable<Prop<T>> props, int i) => new(
		props
			.Select(e => new KeyValuePair<string, JsonNode?>(
				e.Name,
				obj.ToJsonValue(e)
			))
			.Prepend(new KeyValuePair<string, JsonNode?>(
				"id",
				JsonValue.Create(i)
			))
	);

	static JsonValue ToJsonValue<T>(this T obj, Prop<T> prop) => JsonValue.Create($"{prop.Get(obj)}");
}




file static class JsonEnumUtils
{
	static readonly JsonSerializerOptions jsonOpt = new()
	{
		WriteIndented = true,
		IndentCharacter = '\t',
		IndentSize = 1,
		Converters =
		{
			new AttributeBasedEnumConverter(),
		},
	};

	public static string Ser<T>(T obj) => JsonSerializer.Serialize(obj, jsonOpt);
}