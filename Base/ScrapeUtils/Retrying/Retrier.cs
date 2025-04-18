﻿using BaseUtils;

namespace ScrapeUtils;

sealed class Retrier
{
	readonly FullStatsKeeper stats = new();

	public FullStats Stats => stats.Compile();

	public async Task Run(
		Func<Task> action,
		string name,
		RetryPolicy policy,
		CancellationToken cancelToken
	)
	{

		var keeper = stats.GetSpotsKeeper(name, policy);
		var result = await RunInternal(policy, action, keeper.TrackRetryException, cancelToken);
		stats.TrackResult(name, policy, result);
		if (result is FailureRetryResult { Ex: var ex })
			throw ex;
	}



	static async Task<IRetryResult> RunInternal(
		RetryPolicy policy,
		Func<Task> action,
		Action<Exception> trackRetryException,
		CancellationToken cancelToken
	)
	{
		for (var i = 1; i <= policy.MaxRetries; i++)
		{
			var isLastTry = i == policy.MaxRetries;

			try
			{
				await action();
				cancelToken.ThrowIfCancellationRequested();
				return RetryResult.Success(i - 1);
			}
			catch (Exception ex) when (!ex.IsCancel())
			{
				if (isLastTry)
				{
					// On the last try, we give up regardless of the exception type
					return RetryResult.Failure(ex);
				}
				else
				{
					// If we have some tries left, we only give up if the exception doesn't match the predicate
					if (policy.Predicate(ex))
					{
						trackRetryException(ex);
						await Task.Delay(policy.Delay, cancelToken);
					}
					else
					{
						return RetryResult.Failure(ex);
					}
				}
			}
		}

		throw new ArgumentException("[Retrier.RunInternal] => Impossible");
	}



	public async Task<T> Return<T>(
		Func<Task<T>> action,
		string name,
		RetryPolicy policy,
		CancellationToken cancelToken
	) where T : class
	{

		var keeper = stats.GetSpotsKeeper(name, policy);
		var (result, resultValue) = await ReturnInternal(policy, action, keeper.TrackRetryException, cancelToken);
		stats.TrackResult(name, policy, result);
		if (result is FailureRetryResult { Ex: var ex })
			throw ex;
		return resultValue ?? throw new ArgumentException("This shouldn't be null on success");
	}



	static async Task<(IRetryResult, T?)> ReturnInternal<T>(
		RetryPolicy policy,
		Func<Task<T>> action,
		Action<Exception> trackRetryException,
		CancellationToken cancelToken
	) where T : class
	{
		for (var i = 1; i <= policy.MaxRetries; i++)
		{
			var isLastTry = i == policy.MaxRetries;

			try
			{
				var resultValue = await action();
				cancelToken.ThrowIfCancellationRequested();
				return (RetryResult.Success(i - 1), resultValue);
			}
			catch (Exception ex) when (!ex.IsCancel())
			{
				if (isLastTry)
				{
					// On the last try, we give up regardless of the exception type
					return (RetryResult.Failure(ex), null);
				}
				else
				{
					// If we have some tries left, we only give up if the exception doesn't match the predicate
					if (policy.Predicate(ex))
					{
						trackRetryException(ex);
						await Task.Delay(policy.Delay, cancelToken);
					}
					else
					{
						return (RetryResult.Failure(ex), null);
					}
				}
			}
		}

		throw new ArgumentException("[Retrier.RunInternal] => Impossible");
	}

}