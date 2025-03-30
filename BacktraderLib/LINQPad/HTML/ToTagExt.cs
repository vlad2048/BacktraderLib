using BacktraderLib._sys.Utils;
using LINQPad.Controls.Core;
using LINQPad.Controls;
using LINQPad;
using System.Reflection;

namespace BacktraderLib;

public static class ToTagExt
{
	public static Tag ToTag<T>(this IObservable<T> source)
	{
		var dc = new DumpContainer();
		source.Subscribe(e => dc.UpdateContent(e)).D(D);
		return dc.ToTag();
	}
	public static Tag ToTag(this DumpContainer dc) => ToTagInner(dc.WrapInDiv());
	public static Tag ToTag(this Control ctrl) => ToTagInner(ctrl);



	static Tag ToTagInner(Control kid)
	{
		var (idDad, idKid) = (IdGen.Make(), IdGen.Make());
		kid.HtmlElement.ID = idKid;

		var dad = new Tag("div")
		{
			Id = idDad,
			OnRender = () =>
			{
				kid.Dump();
			},
		};


		JS.Run(
			"""

			(async () => {
				const [eltDad, eltKid] = await Promise.all([
					window.waitForElement(____0____),
					window.waitForElement(____1____),
				]);
				eltDad.appendChild(eltKid);
			})();
			""",
			e => e
				.JSRepl_Val(0, idDad)
				.JSRepl_Val(1, idKid)
		);


		return dad;
	}



	static readonly FieldInfo HtmlElement_DumpContainer_Field = typeof(HtmlElement).GetField("DumpContainer", BindingFlags.NonPublic | BindingFlags.Instance)!;

	static Div WrapInDiv(this DumpContainer dc)
	{
		var div = new Div();
		HtmlElement_DumpContainer_Field.SetValue(div.HtmlElement, dc);
		return div;
	}
}