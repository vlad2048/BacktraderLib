using BaseUtils;
using RxLib;

namespace BacktraderLib._sys;

sealed class TraceLayoutKeeper
{
	readonly List<ITrace> currentTrace;
	Layout currentLayout;

	readonly Action<(int, ITrace)[], Layout> update;
	readonly IRoVar<bool> isRendered;


	public TraceLayoutKeeper(
		ITrace[] currentTrace,
		Layout currentLayout,
		IRoVar<bool> isRendered,
		Action<(int, ITrace)[], Layout> update
	)
	{
		(this.currentTrace, this.currentLayout, this.isRendered, this.update) = (currentTrace.ToList(), currentLayout, isRendered, update);
		isRendered.Where(e => e).Subscribe(_ => OnRendered()).D(D);
	}

	public void OnUpdate((int, ITrace)[] updatesTrace, Layout updateLayout)
	{
		CreateMissingTraces(currentTrace, updatesTrace);
		if (!isRendered.V)
		{
			// Before the element is rendered: Keep track and accumulate the updates
			// -------------------------------
			Merge_Trace(currentTrace, updatesTrace);
			Merge_Layout(ref currentLayout, updateLayout);
		}
		else
		{
			// After the element is rendered: Send the updates directly to Plotly
			// ------------------------------
			update(updatesTrace, updateLayout);
		}
	}


	// Step 1
	// ======
	// Return and clear the current state of the traces to call Plotly.NewPlot
	public (ITrace[], Layout) OnDump()
	{
		var resTrace = GetAndEmpty_Trace(currentTrace);
		var resLayout = GetAndEmpty_Layout(ref currentLayout);
		return (resTrace, resLayout);
	}

	// Step 2
	// ======
	// If any updates were accumulated between OnDump() and OnRendered(),
	// return those to call Plotly.Restyle
	//
	// isRendered = true afterwards (From now on, we stop keeping track of accumulative updates and instead directly call Plotly.Restyle)
	void OnRendered()
	{
		update(
			currentTrace
				.Index()
				.Where(t => t.Item.PlotlySer() != new Layout().PlotlySer())
				.SelectA(t => (t.Index, t.Item)),
			currentLayout
		);
	}


	static void CreateMissingTraces(List<ITrace> cur, (int, ITrace)[] upd)
	{
		if (upd.Length == 0) return;
		var lngCur = cur.Count;
		var lngUpd = upd.Max(e => e.Item1) + 1;
		var add = Math.Max(0, lngUpd - lngCur);
		if (add > 0)
			throw new ArgumentException("Cannot update non existing traces");
	}

	static void Merge_Trace(List<ITrace> cur, (int, ITrace)[] upd)
	{
		foreach (var (idx, tr) in upd)
			cur[idx] = ObjectMerger.Merge(cur[idx], tr);
	}

	static void Merge_Layout(ref Layout cur, Layout upd) => cur = ObjectMerger.Merge(cur, upd);


	static ITrace[] GetAndEmpty_Trace(List<ITrace> cur)
	{
		var res = cur.ToArray();
		//for (var i = 0; i < cur.Count; i++)
		//	cur[i] = new ScatterTrace(); // TODO: is this right?
		return res;
	}

	static Layout GetAndEmpty_Layout(ref Layout cur)
	{
		var res = ObjectMerger.Merge(new Layout(), cur);
		cur = new Layout();
		return res;
	}
}
