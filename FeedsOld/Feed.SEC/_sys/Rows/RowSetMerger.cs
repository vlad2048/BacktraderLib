using System.IO.Compression;
using Feed.SEC._sys.Logic;

namespace Feed.SEC._sys.Rows;


static class RowSetMerger
{
	public static void Merge(string archFile, RowSet rowSetNext)
	{
		if (rowSetNext.IsEmpty)
			return;

		var exists = File.Exists(archFile);
		var rowSetPrev = exists ? RowsLoader.Load([archFile]) : RowSet.Empty;
		var rowSet = RowSetUtils.Merge([rowSetPrev, rowSetNext]);
		if (exists)
			File.Delete(archFile);

		using var arch = ZipFile.Open(archFile, ZipArchiveMode.Create);
		arch.SetRows(rowSet.Nums);
		arch.SetRows(rowSet.Pres);
		arch.SetRows(rowSet.Subs);
		arch.SetRows(rowSet.Tags);
	}

	
	static void SetRows<T>(this ZipArchive arch, T[] rows) where T : IRow<T>
	{
		var entry = arch.CreateEntry(T.NameInArchive);
		using var stream = entry.Open();
		using var bs = new BufferedStream(stream);
		using var sw = new StreamWriter(bs);
		sw.WriteLine(T.HeaderLine);
		foreach (var row in rows)
			sw.WriteLine(LineIO.Fields2Line(row.Fields, LineMethod.ReplaceTabChar));
	}
}