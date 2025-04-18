using BaseUtils;

namespace Feed.Universe;


public enum ExchangeName
{
	NASDAQ,
	NYSE,
}

public enum IndexName
{
	NASDAQ100,
	SP500,
	DowJones,
}


public interface IUniverse;

public sealed record ExchangeUniverse(ExchangeName Name) : IUniverse
{
	public override string ToString() => $"{Name}";
	public object ToDump() => ToString();
}

public sealed record IndexUniverse(IndexName Name) : IUniverse
{
	public override string ToString() => $"{Name}";
	public object ToDump() => ToString();
}

public static class Universe
{
	public static readonly IUniverse[] AllExchanges = Enum.GetValues<ExchangeName>().SelectA(Exchange);
	public static readonly IUniverse[] AllIndices = Enum.GetValues<IndexName>().SelectA(Index);
	public static readonly IUniverse[] All = AllExchanges.ConcatA(AllIndices);

	public static IUniverse Exchange(ExchangeName name) => new ExchangeUniverse(name);
	public static IUniverse Index(IndexName name) => new IndexUniverse(name);
}



static class UniverseUtils
{
	const string CountryUS = "United States";

	public static ExchangeName[] GetExchanges(this IUniverse universe) =>
		universe switch
		{
			ExchangeUniverse { Name: var exchangeName } => [exchangeName],
			IndexUniverse { Name: IndexName.NASDAQ100 } => [ExchangeName.NASDAQ],
			IndexUniverse { Name: IndexName.SP500 } => [ExchangeName.NASDAQ, ExchangeName.NYSE],
			IndexUniverse { Name: IndexName.DowJones } => [ExchangeName.NASDAQ, ExchangeName.NYSE],
			_ => throw new ArgumentException($"Unknown Universe: {universe}"),
		};

	public static string[] GetCountries(this IUniverse universe) =>
		universe switch
		{
			ExchangeUniverse { Name: ExchangeName.NASDAQ } => [CountryUS],
			ExchangeUniverse { Name: ExchangeName.NYSE } => [CountryUS],
			IndexUniverse { Name: IndexName.NASDAQ100 } => [CountryUS],
			IndexUniverse { Name: IndexName.SP500 } => [CountryUS],
			IndexUniverse { Name: IndexName.DowJones } => [CountryUS],
			_ => throw new ArgumentException($"Unknown Universe: {universe}"),
		};
}