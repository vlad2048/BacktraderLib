namespace BaseUtils;

public static class CancelExceptionUtils
{
	public static bool IsCancel(this Exception ex) =>
		ex switch
		{
			TaskCanceledException => true,
			AggregateException agg when agg.InnerExceptions.Any(e => e is TaskCanceledException) => true,
			_ => false,
		};
}