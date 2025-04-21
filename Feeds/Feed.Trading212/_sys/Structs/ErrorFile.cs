using Feed.Trading212._sys.Utils;

namespace Feed.Trading212._sys.Structs;

sealed record ErrorFile(ExWrap[] Exceptions)
{
	public static ErrorFile Empty => new([]);
}

sealed record ExWrap(
	ExNode Root,
	int Count
);

sealed record ExNode(
	string Type,
	string Message,
	string? Stack,
	ExNode? Kid
);


static class ErrorFileUtils
{
	public static void Log(Exception ex)
	{
		var list = JsonUtils.LoadOr(Consts.Logs.ErrorFile, ErrorFile.Empty).Exceptions.ToList();
		var exRoot = ex.ToExNode();
		var idx = list.Index().FirstOrDefault(t => t.Item.Root.IsSame(exRoot), (-1, null!)).Index;
		if (idx != -1)
			list[idx] = list[idx] with { Count = list[idx].Count + 1 };
		else
			list.Add(new ExWrap(exRoot, 1));
		new ErrorFile([..list]).Save(Consts.Logs.ErrorFile);
	}

	static ExNode ToExNode(this Exception ex) => new(
		ex.GetType().FullName ?? throw new ArgumentException("Failed to get Exception type", ex),
		ex.Message,
		ex.StackTrace,
		ex.InnerException?.ToExNode()
	);

	static bool IsSame(this ExNode a, ExNode b) =>
		a.Type == b.Type &&
		a.Message == b.Message &&
		a.Stack == b.Stack &&
		((a.Kid == null && b.Kid == null) || (a.Kid != null && b.Kid != null && a.Kid.IsSame(b.Kid)));
}