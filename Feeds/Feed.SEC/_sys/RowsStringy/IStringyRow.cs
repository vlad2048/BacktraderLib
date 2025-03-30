namespace Feed.SEC._sys.RowsStringy;


sealed record Loc(string ArchFile, string NameInArchive, int Line)
{
	public override string ToString() => $"{Path.GetFileName(ArchFile)}/{NameInArchive}:{Line}";
	public static readonly Loc Empty = new(string.Empty, string.Empty, 0);
}


interface IStringyRow<out T> where T : IStringyRow<T>
{
	static abstract string NameInArchive { get; }
	static abstract int ColumnCount { get; }
	static abstract T Parse(Loc loc, string[] xs);
	Loc Loc { get; }
}
