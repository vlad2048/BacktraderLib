using RxLib;

namespace BacktraderLib._sys.Events_;


static class EventMaker
{
	sealed record Id(string EventName, string EltId)
	{
		public string EventEltName => JSEventHandlerWriter.GetEventInstanceName(EventName, EltId);
	}


	public static IObservable<T> Make<T>(
		this EventDef def,
		string eltId,
		IRoVar<bool> isRendered
	)
	{
		var Log = Logger.Make(LogCategory.PlotEvents);

		var id = new Id(def.Name, eltId);
		var isOn = false;
		var subj = new Subject<T>().D(D);

		Events.Listen(id.EventEltName, str =>
		{
			var arg = PlotlyJson.SafeDeser<T>(str, $"\"{def.Name}\"");
			Log($"Received: {arg}");
			subj.OnNext(arg);
		});



		void SetIsOn(bool v)
		{
			Log($"SetIsOn({v})");
			if (v == isOn) return;

			if (!isRendered.V)
			{
				// Before the element is rendered: Keep track of the desired subscription status
				// -------------------------------
				isOn = v;
			}
			else
			{
				// After the element is rendered: Subscribe or unsubscribe directly
				// ------------------------------
				if (v)
				{
					Log("CallAdd");
					CallAdd(id);
				}
				else
				{
					Log("CallDel");
					CallDel(id);
				}

				isOn = v;   // not really necessary to keep isOn updated now
			}
		}



		var When =
			Obs.Using(
					() =>
					{
						SetIsOn(true);
						return Disposable.Create(() => SetIsOn(false));
					},
					_ => subj.AsObservable()
				)
				.Publish()
				.RefCount();

		isRendered.Where(e => e).Subscribe(_ =>
		{
			Log($"OnRendered(isOn:{isOn})");
			// OnRender: Subscribe if needed
			// ---------
			if (isOn)
			{
				Log("CallAdd");
				CallAdd(id);
			}
		});


		return When;
	}


	static void CallAdd(Id id) => JS.Run(
		"""
		const elt = document.getElementById(____0____);
		if (elt) elt.on(____1____, ____2____);
		""",
		e => e
			.JSRepl_Val(0, id.EltId)
			.JSRepl_Val(1, id.EventName)
			.JSRepl_Var(2, id.EventEltName)
	);


	static void CallDel(Id id) => JS.Run(
		"""
		const elt = document.getElementById(____0____);
		if (elt) elt.removeListener(____1____, ____2____);
		""",
		e => e
			.JSRepl_Val(0, id.EltId)
			.JSRepl_Val(1, id.EventName)
			.JSRepl_Var(2, id.EventEltName)
	);
}




