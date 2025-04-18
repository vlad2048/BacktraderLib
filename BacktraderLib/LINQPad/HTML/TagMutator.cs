using BaseUtils;
using JetBrains.Annotations;

namespace BacktraderLib;

public interface ITagMutator
{
	ITagMutator SetEnabled(bool enabled);
	ITagMutator SetVisible(bool visible);
	ITagMutator SetClass(string @class);
	ITagMutator SetText(string text);
	ITagMutator SetWidth(string width);
}


sealed class TagMutator(Tag tag) : ITagMutator
{
	readonly List<string> jsLines = new();

	public string JSCode => jsLines.JoinLines();

	public ITagMutator SetEnabled(bool enabled) => Add(
		"elt.disabled = ____0____;",
		e => e.JSRepl_Val(0, !enabled)
	);

	public ITagMutator SetVisible(bool visible) => Add(
		"elt.style.display = ____0____;",
		e => e.JSRepl_Val(0, visible ? "block" : "none")
	);

	public ITagMutator SetClass(string @class) => Add(
		"elt.classList = ____0____;",
		e => e.JSRepl_Val(0, @class)
	);

	public ITagMutator SetText(string text) => Add(
		"elt.innerText = ____0____;",
		e => e.JSRepl_Val(0, text)
	);

	public ITagMutator SetWidth(string width) => Add(
		"elt.style.width = ____0____;",
		e => e.JSRepl_Val(0, width)
	);


	ITagMutator Add(
		[LanguageInjection(InjectedLanguage.JAVASCRIPT)]
		string code,
		Func<string, string>? replFun = null
	)
	{
		jsLines.Add(JS.Fmt(code, replFun));
		return this;
	}
}