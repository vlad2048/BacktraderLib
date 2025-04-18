using Microsoft.Playwright;

namespace ScrapeUtils;

public sealed record LocItem<T>(ILocator Loc, T Item);