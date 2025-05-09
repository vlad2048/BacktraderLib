using BaseUtils;
using LINQPad.Controls;
using RxLib;

namespace BacktraderLib;

public static partial class Ctrls
{
	public static (IRoVar<T>, Tag) ToDropDown<T>(this IRoVar<T[]> Δsource, int initIdx = 0)
	{
		if (Δsource.V.Length == 0) throw new ArgumentException("Empty array not supported");
		var Δrx = Var.Make(Δsource.V[initIdx]);
		var ui = new SelectBox(SelectBoxKind.DropDown, Δsource.V.SelectA(e => (object)$"{e}"), initIdx, c => Δrx.V = Δsource.V[c.SelectedIndex]);
		Δsource.Skip(1).Subscribe(arr =>
		{
			if (arr.Length == 0) throw new ArgumentException("Empty array not supported");
			ui.Options = arr.SelectA(e => (object)$"{e}");
			ui.SelectedIndex = 0;
			Δrx.V = arr[0];
		}).D(D);
		return (Δrx, ui.ToTag());
	}

	public static (IRoVar<T>, Tag) ToDropDown<T>(this T[] source, int initIdx = 0) => Var.MakeConst(source).ToDropDown(initIdx);
}