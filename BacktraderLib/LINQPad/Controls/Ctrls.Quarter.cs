using BaseUtils;
using LINQPad.Controls;
using RxLib;
using Qrt = BaseUtils.Quarter;

namespace BacktraderLib;

public static partial class Ctrls
{
	public static (IRwVar<Qrt>, Tag) Quarter()
	{
		var Δrx = Var.Make(Qrt.MaxValue);

		var listenForChangesFromCode = true;

		var uiCtrl = new SelectBox(
			Qrt.All.ToObjects(),
			Array.IndexOf(Qrt.All, Δrx.V),
			c =>
			{
				listenForChangesFromCode = false;
				Δrx.V = Qrt.All[c.SelectedIndex];
				listenForChangesFromCode = true;
			}
		);
		var ui = uiCtrl.ToTag();

		Δrx
			.Where(_ => listenForChangesFromCode)
			.Subscribe(e => uiCtrl.SelectedIndex = Array.IndexOf(Qrt.All, e)).D(D);

		return (Δrx, ui);
	}
}