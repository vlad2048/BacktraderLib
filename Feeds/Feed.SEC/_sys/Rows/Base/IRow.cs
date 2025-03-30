using Feed.SEC._sys.Logic;
using Feed.SEC._sys.Utils;

namespace Feed.SEC._sys.Rows.Base;


interface IRow<out T> where T : IRow<T>
{
	static abstract string NameInArchive { get; }
	static abstract int ColumnCount { get; }
	static abstract string HeaderLine { get; }
	static abstract T Parse(string[] xs);
	string?[] Fields { get; }
}


static class RowUtils
{
	public static T Parse<T>(string line, LineMethod method) where T : IRow<T> => T.Parse(LineIO.Line2Fields(line, T.ColumnCount, method));
}


sealed record NumPreXRef(
	string Adsh,
	string Tag,
	string Version
)
{
	public override string ToString() => $"[{Adsh} + {$"{Tag}".Ellipse(Consts.DumpCaps.TagInNumPreXRef)}]";
	public object ToDump() => $"{this}";
}