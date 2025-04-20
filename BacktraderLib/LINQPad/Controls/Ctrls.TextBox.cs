using BacktraderLib._sys;
using RxLib;

namespace BacktraderLib;

public static partial class Ctrls
{
	public static (IRoVar<string>, Tag) TextBox(string? value = null, string? placeholder = null)
	{
		value ??= string.Empty;
		var Δrx = Var.Make(value);

		var id = IdGen.Make();
		var ui = new Tag("input", id)
		{
			Attributes =
			{
				{ "type", "text" },
				{ "value", value },
				{ "placeholder", placeholder },
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
				document.getElementById(____0____).value;
				""",
				e => e
					.JSRepl_Val(0, id)
			);
			Δrx.V = str;
		});

		return (Δrx, ui);
	}
}