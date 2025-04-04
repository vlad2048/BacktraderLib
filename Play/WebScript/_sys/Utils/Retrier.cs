namespace WebScript._sys.Utils;

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
		Log("");
		try
		{
			for (var i = 1; i <= policy.MaxRetries; i++)
			{
				void L(string s) => Log($"[Retry {i}/{policy.MaxRetries}] {s}");

				if (i < policy.MaxRetries)
				{
					try
					{
						L("Attempt");
						await fun();
						L("-> Success");
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
			Log("");
		}


		/*var retryCount = 1;

		Log("[Retry] Start");

		while (true)
		{
			try
			{
				Log($"    [Retry {retryCount}/{policy.MaxRetries}] retryCount={retryCount}");
				await fun();
				Log($"    [Retry {retryCount}/{policy.MaxRetries}] Success");
			}
			catch (Exception ex)
			{
				var okCnt = retryCount <= policy.MaxRetries;
				var okEx = policy.Predicate(ex);
				Log($"    [Retry {retryCount}/{policy.MaxRetries}] Error  okCnt:{okCnt}  okEx:{okEx}");
				if (!okCnt || !okEx)
				{
					Log("    We're done => Throw");
					throw;
				}
				else
				{
					Log($"    We try again => Wait({(int)policy.Delay.TotalSeconds}s)");
					await Task.Delay(policy.Delay);
					if (onRetry != null)
					{
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
			}
			finally
			{
				retryCount++;
			}
		}*/
	}


	/*
	public static async Task<T> Retry<T>(
		RetryPolicy policy,
		Func<Task<T>> fun,
		Func<Exception, Task>? onRetry = null
	)
	{
		var retryCount = 1;

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
					if (onRetry != null)
					{
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
			}
			finally
			{
				retryCount++;
			}
		}
	}
	*/
}