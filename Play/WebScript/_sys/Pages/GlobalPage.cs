using System.Text.RegularExpressions;
using BaseUtils;
using WebScript._sys.Structs;
using WebScript.Structs;

namespace WebScript._sys.Pages;

static class GlobalPage
{
	public static async Task SearchGoto(SymbolDef symbol)
	{
		var items = await SearchWithLocs(symbol.Item.FullName);
		var item = items.FirstOrDefault(e => e.Item == symbol.Item);
		if (item == null)
		{
			Log(items.Select(e => e.Item));
			throw new ScrapeException($"Cannot find symbol '{symbol.Item.FullName}' in search results");
		}

		await item.Loc.ClickAsync();
	}

	public static async Task<SearchItem[]> Search(string text)
	{
		var locItems = await SearchWithLocs(text);
		return locItems.SelectA(e => e.Item);
	}
	
	static async Task<LocItem<SearchItem>[]> SearchWithLocs(string text)
	{
		try
		{
			//Log("SearchWithLocs");
			if (await Page.GetByTestId("layout-component").GetByTestId("search-bar").CountAsync() == 0)
			{
				//Log("    [Search] Click on search");
				await Page.GetByTestId("app-header-search-button").ClickAsync();
			}
			else
			{
				//Log("    [Search] Already in search");
			}

			//Log("    [Search] Fill ''");
			await Page.GetByTestId("search-bar").First.FillAsync("");

			//Log("    [Search] Wait 0.5s");
			await Pause(0.5);

			//Log($"    [Search] Fill '{text}'");
			await Page.GetByTestId("search-bar").First.FillAsync(text);

			//Log("    [Search] Wait 2s");
			await Pause(2);

			//Log("    [Search] GetItems");
			var items = await Page.GetByTestId(new Regex("table-search-results")).GetByTestId(new Regex("instrument-search-result-")).AllAsync();
			var list = new List<LocItem<SearchItem>>();
			foreach (var item in items)
			{
				var fullNameLoc = item.GetByTestId("instrument-full-name");
				var fullName = await fullNameLoc.InnerTextAsync();
				var shortName = await item.GetByTestId("short-name").InnerTextAsync();
				var exchange = await item.GetByTestId("exchange").InnerTextAsync();
				list.Add(new LocItem<SearchItem>(new SearchItem(fullName, shortName, exchange), fullNameLoc));
			}

			//Log($"    [Search] GetItems -> {list.Count}");
			return [.. list];
		}
		catch (Exception ex)
		{
			Log(ex);
			throw;
		}
	}
}