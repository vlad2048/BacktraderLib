using System.IO.Compression;
using BaseUtils;
using Feed.SEC._sys.Logic;

namespace Feed.SEC._sys.RowsStringy;

static class StringyRowUtils
{
	const int LineCap = 3;


	public static StringyRowSet ReadRowSet(string archFile, LineMethod method)
	{
		using var arch = ZipFile.OpenRead(archFile);
		var nums = ReadFromArchive<NumStringyRow>(arch, archFile, method);
		var pres = ReadFromArchive<PreStringyRow>(arch, archFile, method);
		var subs = ReadFromArchive<SubStringyRow>(arch, archFile, method);
		var tags = ReadFromArchive<TagStringyRow>(arch, archFile, method);

		return new StringyRowSet(
			nums,
			pres,
			subs,
			tags
		);
	}

	/*public static IEnumerable<T> StreamFromArchive<T>(string archFile, LineMethod method) where T : IStringyRow<T>
	{
		using var arch = ZipFile.OpenRead(archFile);
		var entry = arch.GetEntry(T.NameInArchive) ?? throw new ArgumentException("Invalid NameInArchive");
		using var stream = entry.Open();
		return Read(stream, T.ColumnCount, archFile, T.NameInArchive, method)
			.Select(e => T.Parse(e.Item1, e.Item2));
	}*/


	public static void PrintLines(Loc[] locs, int columnCount, Action<string> Log)
	{
		var (archFile, nameInArchive) = (locs.Select(e => e.ArchFile).Distinct().Single(), locs.Select(e => e.NameInArchive).Distinct().Single());
		Log($"    In {archFile}/{nameInArchive} (lines: {locs.Length})");
		var lines = GetLines(locs.ToHashSet(), columnCount);
		foreach (var (lineIdx, line) in lines.Take(LineCap))
			Log($"        [{lineIdx}] {line}");
		if (locs.Length > LineCap)
			Log("        (...)");
	}




	static T[] ReadFromArchive<T>(ZipArchive arch, string archFile, LineMethod method) where T : IStringyRow<T>
	{
		var entry = arch.GetEntry(T.NameInArchive) ?? throw new ArgumentException("Invalid NameInArchive");
		using var stream = entry.Open();
		var rows = Read(stream, T.ColumnCount, archFile, T.NameInArchive, method).SelectA(e => T.Parse(e.Item1, e.Item2));
		return rows;
	}




	static IEnumerable<(int, string)> GetLines(HashSet<Loc> locs, int columnCount)
	{
		var (archFile, nameInArchive) = (locs.Select(e => e.ArchFile).Distinct().Single(), locs.Select(e => e.NameInArchive).Distinct().Single());
		using var arch = ZipFile.OpenRead(archFile);
		var entry = arch.GetEntry(nameInArchive) ?? throw new ArgumentException("Invalid NameInArchive");
		using var stream = entry.Open();
		foreach (var (loc, xs) in Read(stream, columnCount, archFile, nameInArchive, LineMethod.OriginalEscaping))
			if (locs.Contains(loc))
				yield return (loc.Line, string.Join('\t', xs));
	}

	static IEnumerable<(Loc, string[])> Read(Stream stream, int columnCount, string archFile, string nameInArchive, LineMethod method)
	{
		using var bs = new BufferedStream(stream);
		using var sr = new StreamReader(bs);
		var isFirst = true;
		var lineIdx = 0;
		while (sr.ReadLine() is { } line)
		{
			lineIdx++;
			if (isFirst)
			{
				isFirst = false;
				continue;
			}

			var xs = LineIO.Line2Fields(line, columnCount, method);
			var loc = new Loc(archFile, nameInArchive, lineIdx);
			if (xs.Length != columnCount)
				throw new InvalidOperationException($"Invalid column count @ {loc}");
			yield return (loc, xs);
		}
	}
}