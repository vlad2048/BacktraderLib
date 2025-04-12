using System.Linq.Expressions;
using System.Reflection;
using BaseUtils;
using LINQPad;
using LINQPad.Controls;
using RxLib;

namespace BacktraderLib._sys.ListSelector;

static class ListSelector
{
	public static (IRoVar<S>, Tag) Make<T, S, U>(
		IRoVar<T[]> Δsource,
		Func<T, S>? selFun,
		Func<T, U> dispFun,
		int? pageSize,
		Expression<Func<T, object>>[]? orderFuns,
		Func<T, string>? searchFun
	) where T : class
	{
		if (Δsource.V.Length == 0) throw new ArgumentException("Empty array not supported");

		var Δrx = selFun switch
		{
			not null => Var.Make(Δsource.V[0]),
			null => null,
		};
		var uiCtrls = new List<Tag>();
		
		var ΔsourceOrdered = BuildOrdering(Δsource, orderFuns, uiCtrls);
		var ΔsourceOrderedSearched = BuildSearching(ΔsourceOrdered, searchFun, uiCtrls);
		var ΔsourceOrderedSearchedPaged = BuildPaging(ΔsourceOrderedSearched, pageSize, uiCtrls);
		
		var uiList = Format(ΔsourceOrderedSearchedPaged, dispFun, Δrx).ToDC().ToTag();

		var ui =
			vert([
				horzTight([..uiCtrls]),
				uiList,
			]);

		return (
			selFun switch
			{
				not null => Var.Expr(() => selFun(Δrx.V)),
				null => null!,
			},
			ui
		);
	}

	static Tag horzTight(Tag[] kids) => new("div")
	{
		Style =
		[
			"display: flex",
			"column-gap: 5px",
			"font-size: 12px",
		],
		Kids = kids,
	};




	// **************
	// **************
	// ** Ordering **
	// **************
	// **************
	enum OrdDir { Asc, Desc }
	sealed class Ord<T>
	{
		public bool IsNone { get; }
		public string Name { get; }
		public OrdDir Dir { get; }
		public Func<T, object> Fun { get; }
		public Ord(Expression<Func<T, object>> expr, OrdDir dir)
		{
			IsNone = false;
			(Name, Fun) = ExprUtils.GetNameAndGetter(expr);
			Dir = dir;
		}
		public Ord()
		{
			IsNone = true;
			Name = "None";
			Dir = OrdDir.Asc;
			Fun = null!;
		}
		public override string ToString() => IsNone switch
		{
			true => "None",
			false => $"{Name} " + $"{Dir}".ToLower(),
		};
		public T[] Order(T[] source) => IsNone switch
		{
			true => source,
			false => Dir switch
			{
				OrdDir.Asc => source.OrderBy(Fun).ToArray(),
				OrdDir.Desc => source.OrderByDescending(Fun).ToArray(),
				_ => throw new ArgumentException()
			}
		};
	}
	static IRoVar<T[]> BuildOrdering<T>(IRoVar<T[]> Δsource, Expression<Func<T, object>>[]? orderFuns, List<Tag> ctrls)
	{
		if (orderFuns == null)
			return Δsource;

		var orderings = orderFuns.MakeOrds();
		var Δordering = Var.Make(orderings[0]);
		var tagCtrl = new SelectBox(orderings, 0, c => Δordering.V = orderings[c.SelectedIndex])
		{
			Styles =
			{
				["width"] = "50px",
			},
		};
		ctrls.Add(tagCtrl.ToTag());
		return Var.Expr(() => Δsource.V.OrderWith(Δordering.V));
	}
	static Ord<T>[] MakeOrds<T>(this Expression<Func<T, object>>[] orderFuns) =>
		orderFuns
			.SelectMany(e => new[]
			{
				new Ord<T>(e, OrdDir.Asc),
				new Ord<T>(e, OrdDir.Desc),
			})
			.Prepend(new Ord<T>())
			.ToArray();
	static T[] OrderWith<T>(this T[] source, Ord<T> ord) => ord.Order(source);




	// ***************
	// ***************
	// ** Searching **
	// ***************
	// ***************
	static IRoVar<T[]> BuildSearching<T>(IRoVar<T[]> Δsource, Func<T, string>? searchFun, List<Tag> ctrls)
	{
		if (searchFun == null)
			return Δsource;
		var Δsearch = Var.Make(string.Empty);
		var tagCtrl = new TextBox(Δsearch.V, "30em", c => Δsearch.V = c.Text)
		{
			Styles =
			{
				["width"] = "70px",
			},
		};
		ctrls.Add(tagCtrl.ToTag());
		return Δsource.FilterOn(Δsearch, searchFun);
	}
	static IRoVar<T[]> FilterOn<T>(this IRoVar<T[]> list, IRoVar<string> searchText, Func<T, string> searchFun) =>
		Var.Expr(() => list.V.WhereA(e => IsMatch(searchFun(e), searchText.V)));
	static bool IsMatch(string itemStr, string searchStr) =>
		searchStr
			.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
			.All(part => itemStr.Contains(part, StringComparison.InvariantCultureIgnoreCase));




	// ************
	// ************
	// ** Paging **
	// ************
	// ************
	static IRoVar<T[]> BuildPaging<T>(IRoVar<T[]> Δsource, int? pageSize, List<Tag> ctrls)
	{
		if (!pageSize.HasValue)
			return Δsource;
		var pageSizeVal = pageSize.Value;
		var ΔpageCount = Var.Expr(() => CalcPageCount(Δsource.V.Length, pageSizeVal));
		var ΔpageIdx = Var.Make(0);
		ΔpageCount.Subscribe(e => { if (ΔpageIdx.V >= e) ΔpageIdx.V = e - 1; }).D(D);
		var obs = Observable.Merge(ΔpageIdx.ToUnit(), ΔpageCount.ToUnit());

		var tag = new Span([
			new Span().React(obs, (c, _) => { c.Text = $"{ΔpageIdx.V + 1}/{ΔpageCount.V}"; }),
			new Button("-", _ => ΔpageIdx.V--).React(obs, (c, _) => c.Enabled = ΔpageIdx.V > 0),
			new Button("+", _ => ΔpageIdx.V++).React(obs, (c, _) => c.Enabled = ΔpageIdx.V < ΔpageCount.V - 1),
			new Span().React(Δsource, (c, items) => { c.Text = $"(x{items.Length})"; }),
		]).WithHorizGap(5).ToTag();

		ctrls.Add(tag);

		return Δsource.Page(ΔpageIdx, pageSizeVal);
	}
	static int CalcPageCount(int total, int pageSize) => total switch
	{
		0 => 1,
		_ => Math.Max(1, (total - 1) / pageSize + 1),
	};
	static IRoVar<T[]> Page<T>(this IRoVar<T[]> list, IRoVar<int> pageIndex, int pageSize) => Var.Expr(() => list.V.Skip(pageIndex.V * pageSize).Take(pageSize).ToArray());
	static C React<C, T>(this C c, IObservable<T> rxV, Action<C, T> action) where C : Control
	{
		rxV.Subscribe(v => action(c, v)).D(D);
		return c;
	}
	static C WithHorizGap<C>(this C ctrl, int gap) where C : Control
	{
		ctrl.Styles["display"] = "flex";
		ctrl.Styles["column-gap"] = $"{gap}px";
		ctrl.Styles["align-items"] = "center";
		return ctrl;
	}




	// ****************
	// ****************
	// ** Formatting **
	// ****************
	// ****************
	sealed record Row<T>(T Obj, Tag[] Cells);
	static IRoVar<Tag> Format<T, U>(IRoVar<T[]> Δsource, Func<T, U> dispFun, IRwVar<T>? Δrx) where T : class
	{
		var props = GetProps<U>();
		var Δtable = Δrx switch
		{
			not null => Var.Expr(() => MakeTable(props, Δsource.V.SelectA(e => new Row<T>(e, dispFun(e).GetRowTags(props))), Δrx.V, Δrx)),
			null => Var.Expr(() => MakeTable(props, Δsource.V.SelectA(e => new Row<T>(e, dispFun(e).GetRowTags(props))), null, null)),
		};
		return Δtable;
	}



	static Tag MakeTable<T, U>(Prop<U>[] props, Row<T>[] rows, T? rxVal, IRwVar<T>? Δrx) where T : class =>
		Δrx switch
		{
			not null => MakeTableWithSelect(props, rows, rxVal, Δrx),
			null => MakeTableWithoutSelect(rows),
		};

	static Tag MakeTableWithoutSelect<T>(Row<T>[] rows) where T : class =>
		new DumpContainer(rows.SelectA(e => e.Obj)).ToTag();


	//static Tag MakeTable<T, U>(Prop<U>[] props, Row<T>[] rows, T? rxVal, IRwVar<T>? Δrx) where T : class =>
	//	MakeTableWithSelect(props, rows, rxVal, Δrx);

	static Tag MakeTableWithSelect<T, U>(Prop<U>[] props, Row<T>[] rows, T? rxVal, IRwVar<T>? Δrx) where T : class =>
		new("table")
		{
			Kids =
			[
				new Tag("thead")
				{
					Kids =
					[
						new Tag("tr")
						{
							Kids = props.SelectA(e => new Tag("th", e.Name)),
						},
					],
				},

				new Tag("tbody")
				{
					Kids = rows.SelectA(
						row => new Tag("tr")
						{
							Kids = row.Cells,
							OnClick = Δrx switch
							{
								not null => () => Δrx.V = row.Obj,
								null => null,
							},
							Style = [
								..If(rxVal != null, "cursor: pointer"),
								..If(rxVal != null && rxVal.Equals(row.Obj), "background-color:#333"),
							],
						}
					),
				},
			],
		};
	static string[] A(params string[] xs) => xs;
	static string[] If(bool condition, params string[] xs) => condition switch
	{
		false => [],
		true => xs,
	};
	static Tag[] GetRowTags<U>(this U obj, Prop<U>[] props) =>
		props.SelectA(prop => prop.Get(obj).GetCellTag());
	static Tag GetCellTag(this object obj) =>
		obj switch
		{
			Tag e => new Tag("td")
			{
				Kids = [e],
			},
			_ => new Tag("td", $"{obj}"),
		};
	sealed record Prop<U>(string Name, Func<U, object> Get);
	static Prop<U>[] GetProps<U>() =>
		typeof(U)
			.GetProperties(BindingFlags.Instance | BindingFlags.Public)
			.SelectA(e => new Prop<U>(e.Name, o => e.GetValue((U)o)!));
	static DumpContainer ToDC(this IObservable<object> rxUI)
	{
		var dc = new DumpContainer();
		rxUI.Subscribe(ui => dc.UpdateContent(ui)).D(D);
		return dc;
	}
}