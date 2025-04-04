using Microsoft.Playwright;

namespace Feed.Trading212._sys.Structs;

sealed record LocItem<T>(T Item, ILocator Loc);