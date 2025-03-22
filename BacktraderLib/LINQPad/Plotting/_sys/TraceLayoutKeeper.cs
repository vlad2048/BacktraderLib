using BaseUtils;

namespace BacktraderLib._sys;

sealed class TraceLayoutKeeper<T> where T : ITrace, new()
{
	readonly List<T> currentTrace;
	Layout currentLayout;

	readonly Action<(int, T)[], Layout> update;
	readonly IRoVar<bool> isRendered;


	public TraceLayoutKeeper(
		T[] currentTrace,
		Layout currentLayout,
		IRoVar<bool> isRendered,
		Action<(int, T)[], Layout> update
	)
	{
		(this.currentTrace, this.currentLayout, this.isRendered, this.update) = (currentTrace.ToList(), currentLayout, isRendered, update);
		isRendered.Where(e => e).Subscribe(_ => OnRendered()).D(D);
	}

	public void OnUpdate((int, T)[] updatesTrace, Layout updateLayout)
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
	public (T[], Layout) OnDump()
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


	static void CreateMissingTraces(List<T> cur, (int, T)[] upd)
	{
		if (upd.Length == 0) return;
		var lngCur = cur.Count;
		var lngUpd = upd.Max(e => e.Item1) + 1;
		var add = Math.Max(0, lngUpd - lngCur);
		for (var i = 0; i < add; i++)
			cur.Add(new T());
	}

	static void Merge_Trace(List<T> cur, (int, T)[] upd)
	{
		foreach (var (idx, tr) in upd)
			cur[idx] = ObjectMerger.Merge(cur[idx], tr);
	}

	static void Merge_Layout(ref Layout cur, Layout upd) => cur = ObjectMerger.Merge(cur, upd);


	static T[] GetAndEmpty_Trace(List<T> cur)
	{
		var res = cur.ToArray();
		for (var i = 0; i < cur.Count; i++)
			cur[i] = new T();
		return res;
	}

	static Layout GetAndEmpty_Layout(ref Layout cur)
	{
		var res = ObjectMerger.Merge(new Layout(), cur);
		cur = new Layout();
		return res;
	}
}
