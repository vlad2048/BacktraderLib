using BacktraderLib._sys;
using BaseUtils;
using Frames;

namespace BacktraderLib;


public static class Prices
{
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
	/// <param name="folder">One file for each symbol will be writter in this folder</param>
	public static Frame<string, string, Bar> Save(this Frame<string, string, Bar> prices, string folder)
	{
		PriceSaver.Save(prices, folder);
		return prices;
	}
}