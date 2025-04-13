using Feed.SEC._sys.Logic;
using LINQPad;
using System.IO.Compression;

namespace Feed.SEC._sys.RowsStringy;

static class StringyRowsLoader
{
	public static StringyRowSet Load(string[] archFiles, LineMethod method)
	{
		Util.Progress = 0;
		var nums = new List<NumStringyRow>();
		var pres = new List<PreStringyRow>();
		var subs = new List<SubStringyRow>();
		var tags = new List<TagStringyRow>();
		for (var i = 0; i < archFiles.Length; i++)
		{
			var archFile = archFiles[i];
			using var arch = ZipFile.OpenRead(archFile);

			nums.AddRange(LoadFromArch<NumStringyRow>(arch, method, archFile));
			pres.AddRange(LoadFromArch<PreStringyRow>(arch, method, archFile));
			subs.AddRange(LoadFromArch<SubStringyRow>(arch, method, archFile));
			tags.AddRange(LoadFromArch<TagStringyRow>(arch, method, archFile));

			Util.Progress = (int)((i + 1) * 100.0 / archFiles.Length);
		}
		Util.Progress = null;
		return new StringyRowSet([..nums], [..pres], [..subs], [..tags]);
	}


	public static IEnumerable<T> Load<T>(string[] archFiles, LineMethod method) where T : IStringyRow<T>
	{
		Util.Progress = 0;
		for (var i = 0; i < archFiles.Length; i++)
		{
			var archFile = archFiles[i];
			using var arch = ZipFile.OpenRead(archFile);

			foreach (var row in LoadFromArch<T>(arch, method, archFile))
				yield return row;

			Util.Progress = (int)((i + 1) * 100.0 / archFiles.Length);
		}
		Util.Progress = null;
	}



	public static IEnumerable<T> LoadFromArch<T>(ZipArchive arch, LineMethod method, string archFile) where T : IStringyRow<T>
	{
		var entry = arch.GetEntry(T.NameInArchive) ?? throw new ArgumentException($"Invalid NameInArchive: '{T.NameInArchive}'");
		using var stream = entry.Open();
		using var bs = new BufferedStream(stream);
		using var sr = new StreamReader(bs);
		foreach (var row in LoadFromStream<T>(sr, T.ColumnCount, archFile, T.NameInArchive, method))
			yield return row;
	}



	static IEnumerable<T> LoadFromStream<T>(StreamReader sr, int columnCount, string archFile, string nameInArchive, LineMethod method) where T : IStringyRow<T>
	{
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
			var row = T.Parse(loc, xs);
			yield return row;
		}
	}
}