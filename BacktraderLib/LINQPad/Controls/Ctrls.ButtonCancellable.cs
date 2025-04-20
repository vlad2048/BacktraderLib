using BacktraderLib._sys;
using RxLib;

namespace BacktraderLib;

public static partial class Ctrls
{
	public static Tag ButtonCancellable(
		string name,
		Func<CancellationToken, Task> action,
		IObservable<bool>? ΔenableOn = null
	)
	{
		ΔenableOn ??= Var.MakeConst(true);
		var sourcer = new CancelSourcer().D(D);

		return new Tag("button", null, name)
		{
			// ReSharper disable once AsyncVoidLambda
			OnClick = async () =>
			{
				if (!sourcer.ΔIsRunning.V)
				{
					var cancelToken = sourcer.Start();
					try
					{
						await action(cancelToken);
					}
					finally
					{
						sourcer.Finish();
					}
				}
				else
				{
					sourcer.Cancel();
				}
			}
		}
		.Dyna(
			Obs.CombineLatest(sourcer.ΔIsRunning, ΔenableOn, (isRunning, enableOn) => (isRunning, enableOn)),
			(v, t) => (v.isRunning, v.enableOn) switch
			{
				(true, _) => t.SetText("Cancel").SetEnabled(true).SetClass(CtrlsClasses.Ctrl_Button_Cancel),
				(false, true) => t.SetText(name).SetEnabled(true).SetClass(""),
				(false, false) => t.SetText(name).SetEnabled(false).SetClass(""),
			}
		);
	}


	sealed class CancelSourcer : IDisposable
	{
		public void Dispose()
		{
			if (source?.IsCancellationRequested == false)
				source.Dispose();
		}

		readonly IRwVar<bool> ΔisRunning = Var.Make(false);
		CancellationTokenSource? source;

		public IRoVar<bool> ΔIsRunning => ΔisRunning;

		public CancellationToken Start()
		{
			if (source != null) throw new ArgumentException("[CancelSourcer.Start] Invalid state");
			source = new CancellationTokenSource();
			ΔisRunning.V = true;
			return source.Token;
		}

		public void Cancel()
		{
			if (source == null) throw new ArgumentException("[CancelSourcer.Cancel] Invalid state");
			source.Cancel();
			source = null;
			ΔisRunning.V = false;
		}

		public void Finish()
		{
			if (source == null) return;
			source.Dispose();
			source = null;
			ΔisRunning.V = false;
		}
	}
}