global using Obs = System.Reactive.Linq.Observable;
global using static RxLib.MainDispContainer;
global using System.Reactive.Disposables;
global using System.Reactive.Linq;
global using System.Reactive.Subjects;
using RxLib._sys;

namespace RxLib;

public static class RxLibSetup
{
	public static void Init()
	{
		RxInit.Init();
	}
}