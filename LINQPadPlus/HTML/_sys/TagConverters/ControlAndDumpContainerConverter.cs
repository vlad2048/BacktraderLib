using System.Reflection;
using LINQPad;
using LINQPad.Controls;
using LINQPad.Controls.Core;

namespace LINQPadPlus._sys.TagConverters;

static class ControlAndDumpContainerConverter
{
	static readonly FieldInfo HtmlElement_DumpContainer_Field = typeof(HtmlElement).GetField("DumpContainer", BindingFlags.NonPublic | BindingFlags.Instance)!;
	
	
	public static Tag ToTag(this Control ctrl) => ToTagInner(ctrl);
	
	public static Tag ToTag(this DumpContainer dc) => ToTagInner(dc.WrapInDiv());


	static Tag ToTagInner(Control kid)
	{
		kid.HtmlElement.ID = IdGen.Make();

		var dad = tag.Div.onReady(() => kid.Dump());

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
				.JSRepl_Val(0, dad.Id)
				.JSRepl_Val(1, kid.HtmlElement.ID)
		);

		return dad;
	}


	static Div WrapInDiv(this DumpContainer dc)
	{
		var div = new Div();
		HtmlElement_DumpContainer_Field.SetValue(div.HtmlElement, dc);
		return div;
	}
}