﻿using BacktraderLib._sys;
using RxLib;

namespace BacktraderLib;

public static partial class Ctrls
{
	public static (IRoVar<bool>, Tag) CheckBox(bool value)
	{
		var Δrx = Var.Make(value);

		var id = IdGen.Make();
		var ui = new Tag("input", id)
		{
			Attributes =
			{
				{ "type", "checkbox" },
				//{ "name", id },
				{ "checked", $"{value}" },
			},
			OnRenderJS = JS.Fmt(
				"""
				elt.addEventListener('input', evt => {
					window.dispatch(elt.id, {});
				});
				"""),
		};

		Events.ListenFast(id, () =>
		{
			var str = JS.Return(
				"""
				document.getElementById(____0____).checked;
				""",
				e => e
					.JSRepl_Val(0, id)
			);
			Δrx.V = bool.Parse(str);
		});

		return (Δrx, ui);

		/*var ui = new CheckBox(name ?? string.Empty, value, c => Δrx.V = c.Checked)
		{
			CssClass = CtrlsClasses.WidgetMain,
		}.ToTag();
		return (Δrx, ui);*/
	}
}