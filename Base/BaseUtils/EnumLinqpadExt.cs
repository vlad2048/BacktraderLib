using LINQPad;

namespace BaseUtils;

public static class EnumLinqpadExt
{
	public static IEnumerable<T> ShowProgress<T>(this T[] arr)
	{
		Util.Progress = 0;
		for (var i = 0; i < arr.Length; i++)
		{
			yield return arr[i];
			Util.Progress = (int)((i + 1) * 100.0 / arr.Length);
		}
		Util.Progress = null;
	}
}