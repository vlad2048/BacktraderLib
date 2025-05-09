using System.Text;
using Point = Feed.Final.CompanyHistoryPoint;
using Edge = Feed.Final.CompanyHistoryEdge;
using Graph = Feed.Final.CompanyHistoryGraph;
using BacktraderLib;
using BaseUtils;


namespace Feed.Final._sys.CompanyHistoryPartitioning.GraphPrinting;

static class GraphPrinter
{
	public static Tag Print(Graph graph)
	{
		var sb = new StringBuilder();
		var state = new DispState();
		sb.AddStart();
		graph.Edges.ForEach(edge => sb.AddEdge(edge, state));
		sb.AddEnd();
		var str = sb.ToString();
		return new Tag("div", null, str)
		{
			OnRenderJS = "mermaid.init(undefined, '.mermaid');",
		};
	}


	static void AddEdge(this StringBuilder sb, Edge edge, DispState state) => sb.AppendLine($"	{edge.Prev.Fmt(state)} ==> {edge.Next.Fmt(state)}");

	static void AddStart(this StringBuilder sb) => sb.AppendLine(
		$"""
		 <pre class='mermaid'>
		 graph LR
		 {GraphStyles.MermaidStyles}
		 """);

	static void AddEnd(this StringBuilder sb) => sb.AppendLine(
		"""
		</pre>
		""");



	sealed class DispState
	{
		readonly Dictionary<Point, string> names = new();
		readonly HashSet<string> done = new();
		int curIdx;

		public string GetNodeName(Point pt) =>
			names.TryGetValue(pt, out var name) switch
			{
				true => name,
				false => names[pt] = $"n{curIdx++}",
			};

		public bool HasSeen(string name) => !done.Add(name);
	}

	static string Fmt(this Point pt, DispState state)
	{
		var name = state.GetNodeName(pt);
		return state.HasSeen(name) switch
		{
			false => $"""{name}["{pt.Name}{(pt.Date.HasValue ? $"<br>{pt.Date:yyyy-MM-dd}" : "")}"]""",
			true => name,
		};
	}
}


/*
using BacktraderLib;
using BaseUtils;

namespace Feed.Final._sys.CompanyHistoryPartitioning.GraphPrinting;

static class GraphPrinter
{
	public static Tag Print(Graph graph)
	{
		var (xss, links) = graph.ComputeMaps();
		return null!;
	}



	readonly record struct Pt(int X, int Y);
	readonly record struct R(int X, int Y, int Width, int Height);




	sealed record Link(int X1, int Y1, int X2, int Y2)
	{
		public Link Validate()
		{
			if (X1 == -1 || Y1 == -1 || X2 == -1 || Y2 == -1) throw new ArgumentException($"Invalid Link: {this}");
			return this;
		}
	}
	sealed record Maps(
		Point[][] Xss,
		Link[] Links
	);
	static Maps ComputeMaps(this Graph graph)
	{
		var dates = graph.Points.GroupBy(e => e.Date ?? DateOnly.MinValue).OrderBy(e => e.Key).SelectA(e => e.Key);
		var xss = dates.SelectA(t => graph.Points.WhereA(e => (e.Date ?? DateOnly.MinValue) == t));
		var links = graph.Edges.SelectA(e =>
		{
			var x1 = dates.IdxOf(e.Prev.Date ?? DateOnly.MinValue);
			var x2 = dates.IdxOf(e.Next.Date ?? DateOnly.MinValue);
			var y1 = xss[x1].IdxOf(e.Prev);
			var y2 = xss[x2].IdxOf(e.Next);
			return new Link(x1, y1, x2, y2).Validate();
		});
		return new Maps(xss, links);
	}
}
*/