namespace BaseUtils;

public static class WithExt
{
	public static T With<T>(this T obj, Action action)
	{
		action();
		return obj;
	}
}