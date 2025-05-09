global using Obs = System.Reactive.Linq.Observable;
global using static LINQPadPlus.Rx.MainDispContainer;
using LINQPadPlus.Rx;

namespace LINQPadPlus;

public static class LINQPadPlusSetup
{
	public static void Init()
	{
		RxInit.Init();
		JS.Init();
		Events.Init();
	}
}