using BaseUtils;
using LINQPad.Controls;
using RxLib;

namespace BacktraderLib;

public static partial class Ctrls
{
	public static (IRoVar<T>, Tag) ToListBox<T>(this IRoVar<T[]> Δsource)
	{
		if (Δsource.V.Length == 0) throw new ArgumentException("Empty array not supported");
		var Δrx = Var.Make(Δsource.V[0]);
		var ui = new SelectBox(SelectBoxKind.DropDown, Δsource.V.SelectA(e => (object)$"{e}"), 0, c => Δrx.V = Δsource.V[c.SelectedIndex]);
		Δsource.Subscribe(arr =>
		{
			if (arr.Length == 0) throw new ArgumentException("Empty array not supported");
			ui.Options = arr.SelectA(e => (object)$"{e}");
			ui.SelectedIndex = 0;
			Δrx.V = arr[0];
		}).D(D);
		return (Δrx, ui.ToTag());
	}

	public static (IRoVar<T>, Tag) ToListBox<T>(this T[] source) => Var.MakeConst(source).ToListBox();
}