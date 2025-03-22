using JetBrains.Annotations;

namespace BacktraderLib._sys.Events_;

sealed record EventDef(
	string Name,
	[LanguageInjection(InjectedLanguage.JAVASCRIPT)] string? ArgFun = null
);



static class EventDefs
{
	public static readonly EventDef Click = new("plotly_click", ClickArgs.JSCode);


	public static readonly EventDef[] Defs =
	[
		Click,
	];
}