namespace BacktraderLib._sys.Utils;

sealed record PlotlyError(
	string Code,
	string Container,
	int Trace,
	PlotlyErrorPath Path,
	string Astr,
	string Msg
);

public sealed record PlotlyErrorPath(string[]? Path)
{
	public object? ToDump() => Path;
}

static class ErrorReader
{
	public static PlotlyError[] Read(string args) =>
		(args == string.Empty) switch
		{
			true => [],
			false => PlotlyJson.PlotlyDeser<PlotlyError[]>(args),
		};
}