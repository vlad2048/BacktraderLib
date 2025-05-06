using BacktraderLib._sys;
using Frames;

namespace BacktraderLib;


public static class Prices
{
	public static Serie<string> SaveSerie(this Serie<string> df, string file) => PriceSaver.SaveSerie(df, file);
	public static Serie<string> LoadSerie(string file) => PriceLoader.LoadSerie(file);

	public static Frame<string, string> SaveFrame(this Frame<string, string> df, string file) => PriceSaver.SaveFrame(df, file);
	public static Frame<string, string> LoadFrame(string file) => PriceLoader.LoadFrame(file);

	public static Frame<string, string, Bar> SaveFrame2(this Frame<string, string, Bar> df, string file) => PriceSaver.SaveFrame2(df, file);
	public static Frame<string, string, Bar> LoadFrame2(string file) => PriceLoader.LoadFrame2(file);



	/*
	/// <summary>
	/// Load prices from .csv files
	/// </summary>
	/// <param name="globs">Each element is either a filename or a filename containing a wildcard</param>
	/// <returns>Prices contained in those files</returns>
	public static Frame<string, string, Bar> Load(params string[] globs) => PriceLoader.Load(globs.SelectManyA(FileUtils.GetGlobFiles));

	/// <summary>
	/// Save prices to .csv files
	/// </summary>
	/// <param name="prices">Prices to save</param>
	/// <param name="folder">One file for each symbol will be written in this folder. This folder needs to exist</param>
	public static Frame<string, string, Bar> Save(this Frame<string, string, Bar> prices, string folder)
	{
		PriceSaver.Save(prices, folder);
		return prices;
	}
	*/
}