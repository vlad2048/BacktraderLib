namespace Feed.Trading212._sys.Utils;


sealed record RetryPolicy(
	int MaxRetries,
	TimeSpan Delay,
	Func<Exception, bool> Predicate
);


static class Retrier
{
	public static async Task Retry(
		RetryPolicy policy,
		Func<Task> fun
	)
	{
		var log = Logger.Make(LogCategory.Retry);
		var logVerbose = Logger.Make(LogCategory.RetryVerbose);

		logVerbose("");
		try
		{
			for (var i = 1; i <= policy.MaxRetries; i++)
			{
				void LVerbose(string s) => logVerbose($"[Retry {i}/{policy.MaxRetries}] {s}");
				void L(string s) => log($"[Retry {i}/{policy.MaxRetries}] {s}");

				if (i < policy.MaxRetries)
				{
					try
					{
						LVerbose("Attempt");
						await fun();
						LVerbose("-> Success");
						break;
					}
					catch (Exception ex) when (policy.Predicate(ex))
					{
						L($"Handled Exception -> Retry (in {policy.Delay.TotalSeconds}s)");
						await Task.Delay(policy.Delay);
					}
					catch (Exception)
					{
						L("Unhandled Exception -> throw");
						throw;
					}
				}
				else if (i == policy.MaxRetries)
				{
					try
					{
						await fun();
					}
					catch (Exception ex) when (policy.Predicate(ex))
					{
						L("Handled Exception -> throw (too many attempts)");
						throw;
					}
					catch (Exception)
					{
						L("Unhandled Exception -> throw (also too many attempts)");
						throw;
					}
				}
				else
					throw new ArgumentException("[Retry] Impossible");
			}
		}
		finally
		{
			logVerbose("");
		}
	}
}