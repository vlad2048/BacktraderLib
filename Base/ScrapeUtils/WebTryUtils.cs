using Microsoft.Playwright;

namespace ScrapeUtils;

public static class WebTryUtils
{
	public static async Task Sleep(this Web web, double seconds) => await Task.Delay(TimeSpan.FromSeconds(seconds), web.CancelToken ?? CancellationToken.None);


	public static async Task Click(this Web web, ILoc loc, string name, RetryPolicy? policy = null)
	{
		var locv = loc(web.Page);
		await web.TryRun(
			async () =>
			{
				await locv.ClickAsync();
			},
			name,
			policy
		);
	}
	public static async Task Click(this Web web, ILocator loc, string name, RetryPolicy? policy = null)
	{
		await web.TryRun(
			async () =>
			{
				await loc.ClickAsync();
			},
			name,
			policy
		);
	}
	public static async Task ClickNoCheck(this Web web, ILocator loc, string name, RetryPolicy? policy = null)
	{
		await web.TryRun(
			async () =>
			{
				await loc.ClickAsync(new LocatorClickOptions
				{
					Force = true,
				});
			},
			name,
			policy
		);
	}


	public static async Task Click_B_If_A_NotPresent(this Web web, ILoc locA, ILoc locB, string name, RetryPolicy? policy = null)
	{
		var locAv = locA(web.Page);
		var locBv = locB(web.Page);
		await web.TryRun(
			async () =>
			{
				var is_A_Present = await locAv.CountAsync() != 0;
				if (!is_A_Present)
					await locBv.ClickAsync();
			},
			name,
			policy
		);
	}


	public static async Task CheckAOrB_ClickBIfB(this Web web, ILoc locA, ILoc locB, string name, RetryPolicy? policy = null)
	{
		var locAv = locA(web.Page);
		var locBv = locB(web.Page);
		await web.TryRun(
			async () =>
			{
				var check = await locAv.Or(locBv).IsVisibleAsync();
				if (!check)
					throw new TimeoutException($"[CheckAOrB_ClickBIfB - {name}] CheckAOrB failed");
				var click = await locBv.IsVisibleAsync();
				if (click)
					await locBv.ClickAsync();
			},
			name,
			policy
		);
	}


	public static async Task<LocItem<T>[]> ReadItems<T>(
		this Web web,
		ILoc locItems,
		Func<ILocator, Task<LocItem<T>>> readItem,
		string name,
		RetryPolicy? policy = null
	)
	{
		var locItemsv = locItems(web.Page);
		return await web.TryReturn<LocItem<T>[]>(
			async () =>
			{
				var items = await locItemsv.AllAsync();
				var list = new List<LocItem<T>>();
				foreach (var item in items)
				{
					list.Add(await readItem(item));
				}

				if (list.Count == 0)
					throw new TimeoutException($"[ReadItems - {name}] Failed to find any items");

				return [.. list];
			},
			name,
			policy
		);
	}


	public static async Task<LocItem<T>[]> TypeInSearchBarAndReadResults<T>(
		this Web web,
		ILoc locType,
		string text,
		ILoc locItems,
		Func<ILocator, Task<LocItem<T>>> readItem,
		string name,
		RetryPolicy? policy = null
	)
	{
		var locTypev = locType(web.Page);
		var locItemsv = locItems(web.Page);
		return await web.TryReturn<LocItem<T>[]>(
			async () =>
			{
				await locTypev.FillAsync("");
				await web.Sleep(0.5);
				await locTypev.FillAsync(text);
				await web.Sleep(2.0);

				var items = await locItemsv.AllAsync();
				var list = new List<LocItem<T>>();
				foreach (var item in items)
				{
					list.Add(await readItem(item));
				}

				if (list.Count == 0)
					throw new TimeoutException($"[TypeInSearchBarAndReadResults - {name}] Failed to find any items in the search results");

				return [..list];
			},
			name,
			policy
		);
	}
}