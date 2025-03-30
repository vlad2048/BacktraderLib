using System.Text;
using BacktraderLib._sys;
using BacktraderLib._sys.Utils;
using LINQPad;

namespace BacktraderLib;

public class Tag(string tagName, string? text = null)
{
	public string? Id { get; init; }
	public string? Class { get; init; }
	public string[]? Style { get; init; }
	public Dictionary<string, string> Attributes { get; init; } = [];

	public string OnRenderJS
	{
		init
		{
			Id ??= IdGen.Make();
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
			Id ??= IdGen.Make();
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

	public Action? OnClick
	{
		init
		{
			if (value == null) return;
			Id ??= IdGen.Make();
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
		if (Attributes != null)
			foreach (var (key, val) in Attributes)
				sb.Append($" {key}='{val}'");
		sb.Append(">");

		foreach (var kid in Kids)
			sb.Append($"{kid}");

		if (text != null)
			sb.Append(text);

		sb.Append($"</{tagName}>");
		return sb.ToString();
	}
}
