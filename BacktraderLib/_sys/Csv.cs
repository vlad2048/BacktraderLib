namespace BacktraderLib._sys;

sealed record ColIndices(
	int Date,
	int Open,
	int High,
	int Low,
	int Close,
	int Volume
);

static class Csv
{
	public static ColIndices GetColumnIndices(string line0, string file)
	{
		var cols = line0.ToLowerInvariant().Chop();
		var idxDate = cols.Idx("date");
		var idxOpen = cols.Idx("open");
		var idxHigh = cols.Idx("high");
		var idxLow = cols.Idx("low");
		var idxClose = cols.Idx("close");
		var idxVolume = cols.Idx("volume");

		if (idxDate == -1) throw new ArgumentException($"Cannot find column 'Date' in '{file}'");
		if (idxOpen == -1) throw new ArgumentException($"Cannot find column 'Open' in '{file}'");
		if (idxHigh == -1) throw new ArgumentException($"Cannot find column 'High' in '{file}'");
		if (idxLow == -1) throw new ArgumentException($"Cannot find column 'Low' in '{file}'");
		if (idxClose == -1) throw new ArgumentException($"Cannot find column 'Close' in '{file}'");
		if (idxVolume == -1) throw new ArgumentException($"Cannot find column 'Volume' in '{file}'");

		return new ColIndices(idxDate, idxOpen, idxHigh, idxLow, idxClose, idxVolume);
	}

	public static IEnumerable<string[]> Read(string[] lines)
	{
		if (lines.Length == 0) throw new ArgumentException("No lines");
		foreach (var line in lines.Skip(1))
			yield return line.Chop();
	}


	static int Idx(this string[] cols, string name)
	{
		for (var i = 0; i < cols.Length; i++)
			if (cols[i] == name)
				return i;
		return -1;
	}
}