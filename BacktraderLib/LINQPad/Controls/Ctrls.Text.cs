using BacktraderLib._sys;
using LINQPad.Controls;

namespace BacktraderLib;

public static partial class Ctrls
{
	public static (IRoVar<string>, Tag) Text(
		string? name,
		string? value = null,
		int width = 150
	)
	{
		var Δrx = Var.Make(value ?? string.Empty);
		var ui = new TextBox(value ?? string.Empty, $"{width}px", c => Δrx.V = c.Text)
		{
			CssClass = CtrlsClasses.WidgetMain,
		}.ToTag();
		return (Δrx, ui.WithLabel(name));
	}
}