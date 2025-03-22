namespace BacktraderLib;

static class RxInit
{
	public static void Init()
	{
		MainDispField.d?.Dispose();
		MainDispField.d = new Disp("MainDisp");
	}
}

public static class MainDispContainer
{
	public static Disp D => MainDispField.d ?? throw new ArgumentException("You forgot to call BacktraderLibSetup.Init()");
}

file static class MainDispField
{
	public static Disp? d;
}