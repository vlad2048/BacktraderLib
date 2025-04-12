using BacktraderLib._sys;
using LINQPad.Controls;
using RxLib;

namespace BacktraderLib;

public static partial class Ctrls
{
	public static (IRoVar<bool>, Tag) Checkbox(
		string? name,
		bool value
	)
	{
		var Δrx = Var.Make(value);
		var ui = new CheckBox(name ?? string.Empty, value, c => Δrx.V = c.Checked)
		{
			CssClass = CtrlsClasses.WidgetMain,
		}.ToTag();
		return (Δrx, ui);
	}
}