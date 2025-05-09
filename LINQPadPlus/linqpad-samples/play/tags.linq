<Query Kind="Program">
  <Namespace>LINQPadPlus</Namespace>
  <Namespace>LINQPad.Controls</Namespace>
  <Namespace>RxLib</Namespace>
</Query>

#load "..\lib"


void Main()
{
	var Δon = Var.Make(true);
	var isEnabled = false;

	var btn = tag.Button["Btn"].attr("disabled", $"{!isEnabled}");
	var btnToggle = tag.Button["toggle"].listen("click", () => JS.RunOn(btn.Id, "elt => elt.disabled = ____0____", e => e.JSRepl_Val(0, !(isEnabled = !isEnabled))));
	
	btnToggle.p();
	btn.p();
}

void DemoMixed()
{
	var btn = new Button("Btn");
	var dc = new DumpContainer("ABC");

	tag.Div[[
		tag.Div["kid1"],
		new[] {
			(HtmlNode)tag.Div["kid2"],
			tag.Div["kid3"].js("elt => elt.style.backgroundColor = 'red'"),
			btn,
			tag.Input.listen("input", "elt => elt.value", e => e.Dump()),
			"123",
			tag.Button["Click me!"].listen("click", () => dc.UpdateContent("Yes")),
			dc,
			tag.Div["kid4"].onReady(() => "ready!".Dump()),
		}.If(true),
		tag.Div["kid5"],
		"MiddleText",
	]].Dump();
}



/*
public static class ToTagExt
{
	public static Tag ToTag(this Control ctrl) => ToTagInner(ctrl);


	static Tag ToTagInner(Control kid)
	{
		kid.HtmlElement.ID = $"id_{Guid.NewGuid()}".Replace("-", "");
		
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
}
*/







