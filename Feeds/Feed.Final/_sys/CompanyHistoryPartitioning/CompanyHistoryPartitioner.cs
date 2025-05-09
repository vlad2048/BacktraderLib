using BaseUtils;
using LINQPad;
using Change = Feed.Final.CompanyHistoryChange;
using Point = Feed.Final.CompanyHistoryPoint;
using Edge = Feed.Final.CompanyHistoryEdge;
using Graph = Feed.Final.CompanyHistoryGraph;

namespace Feed.Final._sys.CompanyHistoryPartitioning;

static class CompanyHistoryPartitioner
{
	public static Graph[] Partition(this Change[] chgs) =>
		chgs
			.ToPartitions()
			.ToGraphs()
			.Verify(chgs);



	static Graph[] Verify(this Graph[] gs, Change[] chgs)
	{
		if (!gs.SelectMany(e => e.Changes).IsSameInAnyOrder(chgs))
			throw new ArgumentException("[CompanyHistoryPartitioner] Missing Changes");

		var set = new HashSet<Change>();
		foreach (var chg in gs.SelectMany(e => e.Changes))
		{
			if (!set.Add(chg))
			{
				var issues = gs.Where(e => e.Changes.Contains(chg)).Take(5).ToArray();
				chg.Dump();
				issues.Dump();
				throw new ArgumentException("[CompanyHistoryPartitioner] Do not form a partition");
			}
		}

		if (gs.Any(e => !e.IsConnected()))
			throw new ArgumentException("[CompanyHistoryPartitioner] Graph not connected");

		return gs;
	}

	static bool IsConnected(this Graph g)
	{
		var set = new HashSet<Point> { g.Points[0] };
		var done = false;
		while (!done)
		{
			done = true;
			foreach (var edge in g.Edges)
			{
				if (set.Contains(edge.Prev) && set.Add(edge.Next))
					done = false;
				else if (set.Contains(edge.Next) && set.Add(edge.Prev))
					done = false;
			}
		}
		return g.Points.IsSameInAnyOrder(set);
	}



	static Graph[] ToGraphs(this PartitionDef[] ps) => ps.SelectA(e => ToGraph([..e.Changes]));

	static Graph ToGraph(Change[] changes)
	{
		var pts = new HashSet<Point>();
		var edges = new HashSet<Edge>();
		var map = new Dictionary<string, List<Point>>();
		void AddPt(Point pt)
		{
			if (!map.ContainsKey(pt.Name)) map[pt.Name] = new List<Point>();
			if (pts.Add(pt)) map[pt.Name].Add(pt);
		}
		void AddEdge(Point prev, Point next) => edges.Add(new Edge(prev, next));

		foreach (var chg in changes)
		{
			var namePrev = chg.NamePrev;

			var ptNext = new Point(chg.NameNext, chg.Date);

			var ptPrevPots = map.GetValueOrDefault(namePrev, []);
			var ptPrev = ptPrevPots.Count switch
			{
				0 => new Point(namePrev, null),
				_ => ptPrevPots.GroupBy(e => e.Date).OrderBy(e => e.Key).Last().Single(),
			};
			AddPt(ptPrev);
			AddPt(ptNext);
			AddEdge(ptPrev, ptNext);
		}

		var grps = pts
			.GroupBy(e => e.Name)
			.Select(e => e
				.OrderBy(f => f.Date)
				.DistinctBy(f => f.Date)
				.ToArray()
			)
			.WhereA(e => e.Length > 1);

		foreach (var grp in grps)
			for (var i = 0; i < grp.Length - 1; i++)
				AddEdge(grp[i], grp[i + 1]);

		var ptsArr = pts.ToArray();
		var edgesArr = edges.ToArray();

		return new Graph(
			changes,
			ptsArr,
			edgesArr
		);
	}



	static PartitionDef[] ToPartitions(this Change[] xs)
	{
		var ps = new List<PartitionDef>();
		foreach (var x in xs)
		{
			var psMatch = ps.WhereA(e => e.Contains(x.NamePrev) || e.Contains(x.NameNext));
			if (psMatch.Length == 0)
			{
				ps.Add(new PartitionDef(x));
			}
			else
			{
				var p = Merge(ps, psMatch);
				p.Add(x);
			}
		}
		return [..ps];
	}


	static PartitionDef Merge(List<PartitionDef> ps, PartitionDef[] psMatch)
	{
		switch (psMatch.Length)
		{
			case 0:
				throw new ArgumentException("Impossible");
			case 1:
				return psMatch[0];
			default:
				var first = psMatch[0];
				var others = psMatch.Skip(1).ToArray();
				first.Merge(others);
				foreach (var other in others)
					ps.Remove(other);
				return first;
		}
	}


	sealed class PartitionDef
	{
		public readonly List<Change> Changes = new();
		public readonly HashSet<string> Names = new();

		public PartitionDef(Change chg) => Add(chg);

		public bool Contains(string name) => Names.Contains(name);
		public void Add(Change chg)
		{
			Changes.Add(chg);
			Names.Add(chg.NamePrev);
			Names.Add(chg.NameNext);
		}
		public void Merge(IEnumerable<PartitionDef> others)
		{
			foreach (var other in others)
			{
				Changes.AddRange(other.Changes);
				Names.AddRange(other.Names);
			}
		}
	}
}
