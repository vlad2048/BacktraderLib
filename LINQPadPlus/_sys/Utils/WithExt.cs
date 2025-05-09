namespace LINQPadPlus._sys.Utils;

static class WithExt
{
	public static T With<T>(this T obj, Action action, bool condition = true)
	{
		if (condition)
			action();
		return obj;
	}
}