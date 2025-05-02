using BaseUtils;
using Feed.SEC._sys.Utils;
using FeedUtils;

namespace Feed.Universe._sys;

static class CompanyDefsCacher
{
	public static CompanyDef[] Load() =>
		FileUtils.DoesFileExistAndIsRecentEnough(Consts.CompanyDefsCacheFile, Consts.CompanyDefsCacheMaxAge) switch
		{
			true => JsonUtils.Load<CompanyDef[]>(Consts.CompanyDefsCacheFile),
			false => Recompute().Save(Consts.CompanyDefsCacheFile),
		};


	static CompanyDef[] Recompute() => Universe.AllExchanges.SelectManyA(API.LoadUniverse)
		.Where(e => !e.Symbol.Contains("."))
		.GroupBy(e => e.SecCompanyName)
		.Select(e => new CompanyDef(
			e.Key,
			e.Select(f => f.Exchange).Distinct().Single(),
			e.MaxBy(f => f.MarketCap)!.Symbol,
			e.Max(f => f.MarketCap),
			e.Max(f => f.Revenue)
		))
		.OrderByDescending(e => e.MarketCap)
		.ToArray();
}