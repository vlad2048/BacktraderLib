using Microsoft.Playwright;

namespace Feed.SEC._sys.Scraping.Structs;

sealed record LocItem<T>(T Item, ILocator Loc);