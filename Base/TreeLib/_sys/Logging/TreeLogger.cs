using _sys.Logging.Structs;
using PrettyPrinting;
using _sys.Logging.Utils;
using BaseUtils;

namespace _sys.Logging;

static class TreeLogger
{
	public static TxtArray Log<T>(TNod<T> root)
	{
		var layout = Layout(root);
		var treeSz = layout.Values.Union().Size;
        var buffer = Enumerable.Range(0, treeSz.Height).SelectA(_ => new string(' ', treeSz.Width));

        void Print(R r, string s)
        {
	        var sLines = s.SplitLines();
	        for (var i = 0; i < sLines.Length; i++)
	        {
		        var sLine = sLines[i];
		        var line = buffer[r.Y + i];
		        var n = sLine.Length;
		        buffer[r.Y + i] = line[..r.X] + sLine + line[(r.X + n)..];
	        }
        }

        foreach (var (n, r) in layout)
	        Print(r, $"{n.V}");

        ArrowDrawer.Draw(root, layout, (pos, str) => Print(new R(pos, new Sz(str.Length, 1)), str));

        return Txt.Build(t => buffer.ForEach(line => t.WriteLine(line)));
	}


	static IReadOnlyDictionary<TNod<T>, R> Layout<T>(TNod<T> root)
	{
		var rootSz = root.Map(e => e.GetSz().MakeBigger(Consts.Logging.GutterSz));
		var mapBack = rootSz.Zip(root).ToDictionary(e => e.First, e => e.Second);

		var xs = rootSz.SolveXs(Consts.Logging.AlignLevels);
		var ys = rootSz.SolveYs();

		return rootSz.ToDictionary(
			nodSz => mapBack[nodSz],
			nodSz => new R(
				new Pt(
					xs[nodSz],
					ys[nodSz]
				),
				nodSz.V.MakeSmaller(Consts.Logging.GutterSz)
			)
		);
	}



	static Dictionary<TNod<Sz>, int> SolveXs(this TNod<Sz> rootSz, bool alignLevels) =>
		rootSz
			.MapN(sz => sz.V.Width)
			.MapAlignIf(alignLevels, TreeEnumExt.MaxOrZero)
			.MapBack(rootSz, out var mapBack)
			.FoldLDictN<int, int>(
				(n, w) => n + w
			)
			.ShiftTreeMapDown(0)
			.MapKey(e => mapBack[e]);




	static Dictionary<TNod<Sz>, int> SolveYs(this TNod<Sz> rootSz)
	{
		var hcMap = rootSz.FoldRDictN<Sz, int>(
				(n, hcs) => Math.Max(n.Height, hcs.SumOrZero())
			)
			.ShiftTreeMapUp(hcs => hcs.SumOrZero());

		var ys = new Dictionary<TNod<Sz>, int>();

		int Recurse(TNod<Sz> nod, int y)
		{
			// "height of nod"
			var hn = nod.V.Height;

			// "height of kids"
			var hc = hcMap[nod];

			// "total height" of nod and its kids
			var ht = Math.Max(hn, hc);

			// Layout nod and its kids within [y .. y + ht]
			// ================================================
			// (1) ht >= hn     if ht > hn => shift nod      to be in the center of ht
			// (2) ht >= hc     if ht > hc => shift kids to be in the center of ht

			// (1)
			ys[nod] = y + (ht - hn) / 2;    // => RESULT

			// (2)
			y += (ht - hc) / 2;
			foreach (var c in nod.Kids)
				y += Recurse(c, y);

			return ht;
		}

		Recurse(rootSz, 0);

		return ys;
	}



	static Dictionary<TNod<T>, U> FoldLDictN<T, U>(
		this TNod<T> root,
		Func<T, U?, U> fun
	)
		=>
			root.Zip(root.FoldL(fun))
				.ToDictionary(
					e => e.First,
					e => e.Second.V
				);



	static TNod<U> FoldL<T, U>(
		this TNod<T> root,
		Func<T, U?, U> fun
	)
	{
		TNod<U> Recurse(TNod<T> node, U? mayMappedDadVal)
		{
			var mappedNodeVal = fun(node.V, mayMappedDadVal);
			var mappedKids = node.Kids.Select(kid => Recurse(kid, mappedNodeVal));
			var mappedNode = Nod.Make(mappedNodeVal, mappedKids);
			return mappedNode;
		}

		return Recurse(root, default);
	}



	static TNod<T> MapAlignIf<T>(
		this TNod<T> root,
		bool condition,
		Func<IEnumerable<T>, T> alignFun
	) => condition switch
	{
		false => root,
		true => root.MapAlign(alignFun)
	};



	static TNod<T> MapAlign<T>(
		this TNod<T> root,
		Func<IEnumerable<T>, T> alignFun
	)
	{
		var levelArr = root.GetNodesByLevels();
		var levelValues = levelArr.SelectA(ns => alignFun(ns.Select(e => e.V)));
		var levelMap = GetNodeLevelMap(levelArr);
		return root
			.MapN(n => levelValues[levelMap[n]]);
	}



	static Dictionary<TNod<T>, int> GetNodeLevelMap<T>(TNod<T>[][] levelArr)
	{
		var map = new Dictionary<TNod<T>, int>();
		for (var level = 0; level < levelArr.Length; level++)
		{
			var levelNods = levelArr[level];
			foreach (var nod in levelNods)
				map[nod] = level;
		}
		return map;
	}



	static Dictionary<TNod<T>, U> ShiftTreeMapUp<T, U>(
		this Dictionary<TNod<T>, U> map,
		Func<IEnumerable<U>, U> combFun
	)
	{
		var resMap = new Dictionary<TNod<T>, U>();
		foreach (var (n, _) in map)
		{
			resMap[n] = combFun(n.Kids.Select(e => map[e]));
		}
		return resMap;
	}




	static Dictionary<TNod<T>, U> ShiftTreeMapDown<T, U>(
		this Dictionary<TNod<T>, U> map,
		U rootVal
	)
	{
		var resMap = new Dictionary<TNod<T>, U>();
		foreach (var (n, _) in map)
		{
			resMap[n] = n.Dad switch
			{
				null => rootVal,
				not null => map[n.Dad]
			};
		}
		return resMap;
	}



	static TNod<T>[][] GetNodesByLevels<T>(this TNod<T> root)
	{
		var lists = new List<List<TNod<T>>>();
		void AddToLevel(TNod<T> node, int level)
		{
			List<TNod<T>> list;
			if (level < lists.Count)
				list = lists[level];
			else if (level == lists.Count)
				lists.Add(list = []);
			else
				throw new ArgumentException();
			list.Add(node);
		}
		root.ForEachWithLevel(AddToLevel);
		return lists.SelectA(e => e.ToArray());
	}

	static void ForEachWithLevel<T>(this TNod<T> root, Action<TNod<T>, int> action)
	{
		void Recurse(TNod<T> node, int level)
		{
			action(node, level);
			foreach (var kid in node.Kids)
				Recurse(kid, level + 1);
		}
		Recurse(root, 0);
	}


	static Dictionary<TNod<T>, U> FoldRDictN<T, U>(
		this TNod<T> root,
		Func<T, IReadOnlyList<U>, U> fun
	)
		=>
			root.Zip(root.FoldR(fun))
				.ToDictionary(
					e => e.First,
					e => e.Second.V
				);


	static TNod<TDst> MapBack<TSrc, TDst>(this TNod<TDst> rootDst, TNod<TSrc> rootSrc, out Dictionary<TNod<TDst>, TNod<TSrc>> map)
	{
		map = rootDst.Zip(rootSrc).ToDictionary(e => e.First, e => e.Second);
		return rootDst;
	}



	static Sz GetSz<T>(this T v)
	{
		var xs = $"{v}".SplitLines();
		return new Sz(xs.Max(e => e.Length), xs.Length);
	}
}