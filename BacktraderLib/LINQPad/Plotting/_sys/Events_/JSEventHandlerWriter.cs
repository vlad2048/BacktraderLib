using BacktraderLib._sys.Utils;

namespace BacktraderLib._sys.Events_;

static class JSEventHandlerWriter
{
	static readonly string EmptyArgFun = JS.Fmt(
		"""
		return {};
		"""
	);


	public static string GetEventInstanceName(string eventName, string eltId) => $"{eltId}_{eventName}";


	public static string CodeJSEventHandlerFunctions(this IEnumerable<EventDef> defs, string eltId) =>
		defs
			.Select(def => JS.Fmt(
				"""
				function ____0____(evt) {
					function get() {
						____1____
					}
					window.dispatch('____0____', get());
				}
				""",
				e => e
					.JSRepl_Var(0, GetEventInstanceName(def.Name, eltId))
					.JSRepl_Obj(1, def.ArgFun ?? EmptyArgFun)
			))
			.JoinLines();
}