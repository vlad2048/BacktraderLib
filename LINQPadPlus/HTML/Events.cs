using LINQPad;
using LINQPad.Controls;

namespace LINQPadPlus;

static class Events
{
	const string DispatcherDivId = "dispatcher";
	static Div? dispatcherDiv;
	static Div DispatcherDiv => dispatcherDiv ?? throw new ArgumentException("Events.Init() was not called");

	internal static void Init()
	{
		dispatcherDiv = new Div { HtmlElement = { ID = DispatcherDivId } }.Dump();
		JS.Run(
			"""
			____0____ = document.getElementById('____0____');
			function dispatch(dispatchId, evtArg) {
				____0____.dispatchEvent(new CustomEvent(
					dispatchId,
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

	public static void Listen(string dispatchId, Action action)
	{
		DispatcherDiv.HtmlElement.AddEventListener(dispatchId, (_, _) =>
		{
			action();
		});

		/*
		***********
		* WARNING *
		***********
		
		Without this delay we can miss the event sometimes.
	
		Example:
		========
			void Main()
			{
				var Δon = Var.Make(true);

				var btn = tag.Button["Btn"].enable(Δon);
				var btnToggle = tag.Button["toggle"].listen("click", () =>
				{
					Δon.V = !Δon.V;
				});
				
				btnToggle.p();
				btn.p();
			}
		
		
		Also, add these in the Tag constructor:
		=======================================
			sigPreRender.Subscribe(_ => $"[{kids[0].Text}] OnSigPreRender".Dump());
			sigPostRender.Subscribe(_ => $"[{kids[0].Text}] OnSigPostRender".Dump());
		
		*/
		Thread.Sleep(10);
	}
}