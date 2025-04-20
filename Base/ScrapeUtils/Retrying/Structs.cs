using System.Text.Json.Serialization;
using BaseUtils;

namespace ScrapeUtils;


public sealed record RetryPolicy(
	int MaxRetries,
	TimeSpan Delay,
	Func<Exception, bool> Predicate
)
{
	public static readonly RetryPolicy Default = new(
		5,
		TimeSpan.FromSeconds(0.2),
		ex => ex is TimeoutException
	);
}


/// <summary>
/// Keeps track of retry stats for each spot
/// </summary>
/// 
/// <param name="ExceptionsRetry">
/// Exceptions that we retries (match predicate AND haven't reached max tries)
/// - Does not include Cancel exceptions
/// </param>
/// 
/// <param name="ExceptionsFatal">
/// Exceptions that we did not retry (didn't match predicate OR we reached max tries)
/// - Does not include Cancel exceptions
/// </param>
/// 
/// <param name="TryCounts">
/// TryCounts[i] represents how many runs succeded on the i+1th try
/// </param>
/// 
/// <param name="Total">
/// Total number of times we called this spot
/// </param>
/// 
/// <param name="Failures">
/// Total number of times we did not manage to finish despite the retries.
/// Either we ran into an exception that did not match the predicate OR we reached max tries
/// </param>
public sealed record SpotStats(
	Exception[] ExceptionsRetry,
	Exception[] ExceptionsFatal,
	int[] TryCounts,
	int Total,
	int Failures
)
{
	[JsonIgnore]
	public bool HasError => TryCounts.Skip(1).Any(e => e > 0) || Failures > 0;
}

public sealed record FullStats(
	Dictionary<string, SpotStats> Map
)
{
	public object ToDump() => Map
		.Where(kv => kv.Value.HasError)
		.SelectA(kv => new
		{
			Name = kv.Key,
			Succ = kv.Value.TryCounts[0],
			Fail = kv.Value.Failures,
			Total = kv.Value.Total,
			Retries = kv.Value.TryCounts.Skip(1).JoinText(","),
			ExceptionsRetry = kv.Value.ExceptionsRetry.Fmt(),
			ExceptionsFatal = kv.Value.ExceptionsFatal.Fmt(),
		});
}


file static class ExceptionFmt
{
	public static string[] Fmt(this Exception[] exs) => exs.SelectA(ex => ex.Fmt());
	static string Fmt(this Exception ex) =>
		ex.Message
			.SplitInLines()
			.Take(3)
			.JoinText(" ")
			.Replace("Timeout ", "")
			.Replace("   -   - ", " ")
			.Truncate(128);

	static string[] SplitInLines(this string? str) => str == null ? Array.Empty<string>() : str.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
}




public interface IRetryResult;
sealed record SuccessRetryResult(int Tries) : IRetryResult;
sealed record FailureRetryResult(Exception Ex) : IRetryResult;
static class RetryResult
{
	public static IRetryResult Success(int tries) => new SuccessRetryResult(tries);
	public static IRetryResult Failure(Exception ex) => new FailureRetryResult(ex);
}


public sealed class SpotStatsKeeper
{
	readonly List<Exception> exceptionsRetry = [];
	readonly List<Exception> exceptionsFatal = [];
	readonly int[] tryCounts;
	int failures;
	int total;

	public RetryPolicy Policy { get; }

	public SpotStatsKeeper(RetryPolicy policy)
	{
		tryCounts = new int[policy.MaxRetries];
		Policy = policy;
	}

	public void TrackRetryException(Exception ex) => exceptionsRetry.Add(ex);

	public void TrackResult(IRetryResult result)
	{
		switch (result)
		{
			case SuccessRetryResult { Tries: var tries }:
				total++;
				tryCounts[tries]++;
				break;

			case FailureRetryResult { Ex: var ex }:
				total++;
				failures++;
				exceptionsFatal.Add(ex);
				break;

			default:
				throw new ArgumentException("Impossible");
		}
	}

	public SpotStats Compile() => new(
		[..exceptionsRetry],
		[..exceptionsFatal],
		tryCounts,
		total,
		failures
	);
}


public sealed class FullStatsKeeper
{
	readonly Dictionary<string, SpotStatsKeeper> map = new();

	public SpotStatsKeeper GetSpotsKeeper(string name, RetryPolicy policy)
	{
		if (!map.TryGetValue(name, out var keeper))
			keeper = map[name] = new SpotStatsKeeper(policy);
		else if (policy.MaxRetries != keeper.Policy.MaxRetries || policy.Delay != keeper.Policy.Delay)
			throw new ArgumentException("Inconsistent RetryPolicy");
		return keeper;
	}

	public void TrackResult(string name, RetryPolicy policy, IRetryResult result)
	{
		if (!map.TryGetValue(name, out var keeper))
			keeper = map[name] = new SpotStatsKeeper(policy);
		else if (policy.MaxRetries != keeper.Policy.MaxRetries || policy.Delay != keeper.Policy.Delay)
			throw new ArgumentException("Inconsistent RetryPolicy");
		keeper.TrackResult(result);
	}

	public FullStats Compile() => new(map.ToDictionary(kv => kv.Key, kv => kv.Value.Compile()));

	public object ToDump() => Compile();
}