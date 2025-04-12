global using Obs = System.Reactive.Linq.Observable;
global using static RxLib.MainDispContainer;
global using static BacktraderLib.CtrlsUtilsStatic;
global using System.Reactive.Disposables;
global using System.Reactive.Linq;
global using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using BacktraderLib._sys;
using Frames;
using RxLib;

[assembly: InternalsVisibleTo("BacktraderLib.Tests")]
[assembly: InternalsVisibleTo("LINQPadQuery")]

namespace BacktraderLib;

public static class BacktraderLibSetup
{
	public static void Init()
	{
		RxLibSetup.Init();
		Thread.CurrentThread.Name = "MainThread";
		JSInit.Init();
		Events.Init();
		PlottingInit.Init();
		CtrlsInit.Init();
	}

	public static void Dbg_SetFramePrinterCutoffs(int rowMax, int rowSmp, int colMax, int colSmp) => Frame.Dbg_SetFramePrinterCutoffs(rowMax, rowSmp, colMax, colSmp);
}