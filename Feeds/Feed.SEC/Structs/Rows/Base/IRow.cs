namespace Feed.SEC;

public interface IRow<out T> where T : IRow<T>
{
	static abstract string NameInArchive { get; }
	static abstract int ColumnCount { get; }
	static abstract string HeaderLine { get; }
	static abstract T Parse(string[] xs);
	string?[] Fields { get; }
}