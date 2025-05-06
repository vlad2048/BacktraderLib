using BaseUtils;
using FeedUtils;
using Frames;

namespace BacktraderLib._sys;

static class PriceLoader
{
	public static Serie<string> LoadSerie(string file)
	{
		var df = LoadFrame(file);
		return df.First();
	}

	
	
	public static Frame<string, string> LoadFrame(string file)
	{
		var lines = File.ReadAllLines(file);
		if (lines.Length == 0) throw new ArgumentException("No lines");
		var colNames = lines[0].Chop();
		if (!colNames[0].Equals("date", StringComparison.OrdinalIgnoreCase)) throw new ArgumentException($"Cannot find column 'Date' in '{file}'");
		colNames = colNames.Skip(1).ToArray();

		var data = lines.Skip(1).SelectA(line =>
		{
			var xs = line.Chop();
			return new Row(
				DateTime.Parse(xs[0]),
				xs.Skip(1).SelectA(double.Parse)
			);
		});

		return Frame.Make(
			"Prices",
			data.SelectA(e => e.Date),
			colNames.SelectA((colName, colIdx) => (
				colName,
				data.SelectA(e => e.Values[colIdx])
			))
		);
	}



	public static Frame<string, string, Bar> LoadFrame2(string file) => LoadFrame(file).Unflatten();


	static Frame<string, string, Bar> Unflatten(this Frame<string, string> df) =>
		Frame.Make(
			df.Name,
			df.Index,
			df
				.GroupBy(e => e.Name.SplitName().Item1)
				.SelectA(e => (
					e.Key,
					e.SelectA(f => (
						f.Name.SplitName().Item2,
						f.Values
					))
				))
		);


	static (string, Bar) SplitName(this string name)
	{
		var parts = name.Split('_');
		return (parts[0], Enum.Parse<Bar>(parts[1]));
	}


	sealed record Row(DateTime Date, double[] Values);







	/*
	public static Frame<string, string, Bar> Load(string[] files) =>
		PriceBuilder.Build(
			files.SelectA(e => (Path.GetFileNameWithoutExtension(e), LoadFile(e))),
			e => e.Date,
			e => e.Open,
			e => e.High,
			e => e.Low,
			e => e.Close,
			e => e.Volume
		);

	sealed record LoaderBar(DateTime Date, double Open, double High, double Low, double Close, double Volume);

	static LoaderBar[] LoadFile(string file)
	{
		var lines = File.ReadAllLines(file);
		if (lines.Length == 0) throw new ArgumentException("No lines");
		var cols = Csv.GetColumnIndices(lines[0], file);

		var bars = new List<LoaderBar>();
		foreach (var xs in Csv.Read(lines))
		{
			bars.Add(new LoaderBar(
				DateTime.Parse(xs[cols.Date]),
				double.Parse(xs[cols.Open]),
				double.Parse(xs[cols.High]),
				double.Parse(xs[cols.Low]),
				double.Parse(xs[cols.Close]),
				double.Parse(xs[cols.Volume])
			));
		}

		return [..bars];
	}
	*/
}