namespace RxLib._sys;

static class RxInit
{
	public static void Init()
	{
		MainDispField.d?.Dispose();
		MainDispField.d = new Disp("MainDisp");
	}
}