using BacktraderLib._sys;
using BacktraderLib.Utils;

namespace BacktraderLib;

public static class Themes
{
	public static string Dark => dark.Value;
	static readonly Lazy<string> dark = Load("dark");
	


	const string ResourceFolder = "BacktraderLib.LINQPad.Plotting._sys.Themes";

	static Lazy<string> Load(string name) => new(() =>
		ResourceLoader.Load($"{ResourceFolder}.{name}.json")
			.PlotlyJsonMinify()
	);
}