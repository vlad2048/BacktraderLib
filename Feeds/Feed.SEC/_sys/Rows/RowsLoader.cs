using Feed.SEC._sys.Logic;
using LINQPad;
using System.IO.Compression;

namespace Feed.SEC._sys.Rows;

static class RowsLoader
{
	public static RowSet Load(string[] archFiles)
	{
		Util.Progress = 0;
		var nums = new List<NumRow>();
		var pres = new List<PreRow>();
		var subs = new List<SubRow>();
		var tags = new List<TagRow>();
		for (var i = 0; i < archFiles.Length; i++)
		{
			var archFile = archFiles[i];
			using var arch = ZipFile.OpenRead(archFile);

			nums.AddRange(LoadFromArch<NumRow>(arch));
			pres.AddRange(LoadFromArch<PreRow>(arch));
			subs.AddRange(LoadFromArch<SubRow>(arch));
			tags.AddRange(LoadFromArch<TagRow>(arch));

			Util.Progress = (int)((i + 1) * 100.0 / archFiles.Length);
		}
		Util.Progress = null;
		return new RowSet([.. nums], [.. pres], [.. subs], [.. tags]);
	}


	public static IEnumerable<T> Load<T>(string[] archFiles) where T : IRow<T>
	{
		Util.Progress = 0;
		for (var i = 0; i < archFiles.Length; i++)
		{
			var archFile = archFiles[i];
			using var arch = ZipFile.OpenRead(archFile);

			foreach (var row in LoadFromArch<T>(arch))
				yield return row;

			Util.Progress = (int)((i + 1) * 100.0 / archFiles.Length);
		}
		Util.Progress = null;
	}



	public static IEnumerable<T> LoadFromArch<T>(ZipArchive arch) where T : IRow<T>
	{
		var entry = arch.GetEntry(T.NameInArchive) ?? throw new ArgumentException($"Invalid NameInArchive: '{T.NameInArchive}'");
		using var stream = entry.Open();
		using var bs = new BufferedStream(stream);
		using var sr = new StreamReader(bs);
		foreach (var row in LoadFromStream<T>(sr))
			yield return row;
	}



	static IEnumerable<T> LoadFromStream<T>(StreamReader sr) where T : IRow<T>
	{
		var isFirst = true;
		while (sr.ReadLine() is { } line)
		{
			if (isFirst)
			{
				isFirst = false;
				continue;
			}

			var xs = LineIO.Line2Fields(line, T.ColumnCount, LineMethod.ReplaceTabChar);
			if (xs.Length != T.ColumnCount)
				throw new InvalidOperationException($"Invalid column count {xs.Length} != {T.ColumnCount}");
			var row = T.Parse(xs);
			yield return row;
		}
	}
}