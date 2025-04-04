using Microsoft.Playwright;

namespace WebScript._sys.Structs;

sealed record LocItem<T>(T Item, ILocator Loc);