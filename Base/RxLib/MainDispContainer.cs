using RxLib._sys;

namespace RxLib;

public static class MainDispContainer
{
	public static Disp D => MainDispField.d ?? throw new ArgumentException("You forgot to call BacktraderLibSetup.Init()");
}