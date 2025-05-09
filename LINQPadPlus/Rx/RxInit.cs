namespace LINQPadPlus.Rx;

static class RxInit
{
	public static Disp? d;

	public static void Init()
	{
		d?.Dispose();
		d = new Disp();
	}
}


static class MainDispContainer
{
	public static Disp D => RxInit.d ?? throw new ArgumentException("You forgot to call BacktraderLibSetup.Init()");
}
