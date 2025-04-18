using LINQPad;
using LINQPad.Controls;

namespace BacktraderLib._sys;

// HACK public
public static class Events
{
	const string DispatcherDivId = "dispatcher";
	static Div? dispatcherDiv;
	internal static Div DispatcherDiv => dispatcherDiv ?? throw new ArgumentException("Events.Init() was not called");

	internal static void Init()
	{
		dispatcherDiv = new Div { HtmlElement = { ID = DispatcherDivId } }.Dump();
		JS.Run(
			"""
			____0____ = document.getElementById('____0____');
			function dispatch(evtName, evtArg) {
				____0____.dispatchEvent(new CustomEvent(
					evtName,
					{
						detail: evtArg
					}
				));
			}
			""",
			e => e
				.JSRepl_Var(0, DispatcherDivId)
		);
	}

	public static void Listen(string evtName, Action<string> action)
	{
		DispatcherDiv.HtmlElement.AddEventListener(evtName, ["detail"], (_, e) => action(e.ReadProp()));

		// HACK
		// In some rare cases, it's possible that the event listener we just set
		// won't be ready by the time we call dispatch() causing us to miss the event completely.
		//
		// - I haven't seen any issues with 10ms
		// - I haven't tested any smaller delays
		Thread.Sleep(10);
	}

	public static void ListenFast(string evtName, Action action)
	{
		DispatcherDiv.HtmlElement.AddEventListener(evtName, (_, _) =>
		{
			action();
		});
	}

	static string ReadProp(this PropertyEventArgs args)
	{
		if (!args.Properties.TryGetValue("detail", out var str)) throw new ArgumentException("(Impossible) Detail property not found in custom event");
		return str;
	}
}