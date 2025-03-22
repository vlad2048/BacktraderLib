using BaseUtils;
using FeedUtils;
using Frames;

namespace BacktraderLib._sys;

static class PriceLoader
{
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
}