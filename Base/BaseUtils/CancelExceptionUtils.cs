namespace BaseUtils;

public static class CancelExceptionUtils
{
	public static bool IsCancel(this Exception ex) =>
		ex switch
		{
			OperationCanceledException => true,
			AggregateException agg when agg.InnerExceptions.Any(e => e is OperationCanceledException) => true,
			_ => false,
		};
}