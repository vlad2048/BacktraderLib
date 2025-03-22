using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using BacktraderLib._sys;
using BacktraderLib._sys.Utils;
using LINQPad;
using LINQPad.Controls;
using LINQPad.Controls.Core;

namespace BacktraderLib;

public class Tag(string tagName, string? text = null)
{
	public string? Id { get; init; }
	public string? Class { get; init; }
	public string[]? Style { get; init; }

	public string OnRenderJS
	{
		init
		{
			if (Id == null) throw new ArgumentException("Id needs to be set before using OnRenderJS");
			JS.Run(
				"""
				(async () => {
					const elt = await window.waitForElement(____0____);
					____1____
				})();
				""",
				e => e
					.JSRepl_Val(0, Id)
					.JSRepl_Obj(1, value)
			);
		}
	}

	public Action OnRender
	{
		init
		{
			if (Id == null) throw new ArgumentException("Id needs to be set before using OnRender");
			var evtName = $"{Id}_OnRender";
			Events.Listen(evtName, _ => value());
			JS.Run(
				"""
				(async () => {
					const elt = await window.waitForElement(____0____);
					window.dispatch(____1____, '');
				})();
				""",
				e => e
					.JSRepl_Val(0, Id)
					.JSRepl_Val(1, evtName)
			);
		}
	}

	public Action OnClick
	{
		init
		{
			if (Id == null) throw new ArgumentException("Id needs to be set before using OnClick");
			var evtName = $"{Id}_OnClick";
			Events.ListenFast(evtName, value);
			JS.Run(
				"""
				(async () => {
					const elt = await window.waitForElement(____0____);
					elt.addEventListener('click', () => {
						window.dispatch(____1____, '');
					});
				})();
				""",
				e => e
					.JSRepl_Val(0, Id)
					.JSRepl_Val(1, evtName)
			);
		}
	}

	public Tag[] Kids { get; init; } = [];


	public object ToDump()
	{
		if (Kids.Length > 0 && text != null) throw new ArgumentException("Cannot use both Kids and text");
		return Util.RawHtml($"{this}");
	}


	public override string ToString()
	{
		var sb = new StringBuilder();
		sb.Append($"<{tagName}");
		if (Id != null)
			sb.Append($" id='{Id}'");
		if (Class != null)
			sb.Append($" class='{Class}'");
		if (Style != null)
			sb.Append($" style='{string.Join(';', Style)}'");
		sb.Append(">");

		foreach (var kid in Kids)
			sb.Append($"{kid}");

		if (text != null)
			sb.Append(text);

		sb.Append($"</{tagName}>");
		return sb.ToString();
	}
}




public static class TagExt
{
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