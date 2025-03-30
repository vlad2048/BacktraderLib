namespace Feed.SEC._sys.Scraping.Utils;

sealed record RetryPolicy(
	int MaxRetries,
	TimeSpan Delay,
	Func<Exception, bool> Predicate
);


static class Retrier
{
	public static async Task<T> Retry<T>(
		RetryPolicy policy,
		Func<Task<T>> fun,
		Func<Exception, Task> onRetry,
		Action<object> log
	)
	{
		var retryCount = 1;

		void Log(string s)
		{
			log($"[{DateTime.Now:HH:mm:ss.fff}] {s}");
		}

		while (true)
		{
			try
			{
				var result = await fun();
				return result;
			}
			catch (Exception ex)
			{
				var okCnt = retryCount <= policy.MaxRetries;
				var okEx = policy.Predicate(ex);
				Log($"Retry {retryCount}/{policy.MaxRetries}");
				Log($"    Error  okCnt:{okCnt}  okEx:{okEx}");
				if (!okCnt || !okEx)
				{
					Log("    We're done => Throw");
					throw;
				}
				else
				{
					Log($"    We try again => Wait({(int)policy.Delay.TotalSeconds}s)");
					await Task.Delay(policy.Delay);
					try
					{
						await onRetry(ex);
					}
					catch (Exception exRetry)
					{
						Log($"  Exception on retry, ignore it: [{exRetry.GetType().Name} / {exRetry.Message}]");
					}
				}
			}
			finally
			{
				retryCount++;
			}
		}
	}
}