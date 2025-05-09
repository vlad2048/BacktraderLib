using BacktraderLib._sys.Events_;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using BaseUtils;

namespace BacktraderLib._sys;

static class Plotly
{
	// Inherit from Tag
	// ----------------
	public static string NewPlot<T>(
		string eltId,
		T[] traces,
		Layout layout,
		Config config
	) where T : ITrace
	{
		var js = JS.Fmt(
			"""

			____0____

			(async () => {
				const plot = await Plotly.newPlot(
					____1____,
					____2____,
					____3____,
					____4____
				);
				//try {
				//	const errs = Plotly.validate(plot.data, plot.layout);
				//	window.dispatch(elt.id, errs);
				//}
				//catch (error) {
				//	// log('Error calling Plotly.validate');
				//	// log(error);
				//	window.dispatch(elt.id, []);
				//}

			})();
			""",
			e => e
				.JSRepl_Obj(0, EventDefs.Defs.CodeJSEventHandlerFunctions(eltId))
				.JSRepl_Val(1, eltId)
				.JSRepl_ArrOfObj(2, traces)
				.JSRepl_Obj(3, layout.PlotlySer())
				.JSRepl_Obj(4, config.PlotlySer())
		);

		return js;
	}

	static string JSRepl_ArrOfObj<T>(this string c, int i, T[] xs) => c.JSRepl_Obj(i, xs.PlotlySer());



	/*
	// ToDump() -> Util.RawHtml
	// ------------------------
	public static string NewPlot<T>(
		string eltId,
		T[] traces,
		Layout layout,
		Config config
	) where T : ITrace
	{
		var js = JS.Fmt(
			"""
			
			____0____
			
			(async () => {
				const elt = await window.waitForElement(____1____);
				const plot = await Plotly.newPlot(
					elt,
					____2____,
					____3____,
					____4____
				);
				try {
					const errs = Plotly.validate(plot.data, plot.layout);
					window.dispatch(elt.id, errs);
				}
				catch (error) {
					// log('Error calling Plotly.validate');
					// log(error);
					window.dispatch(elt.id, []);
				}
			
			})();
			""",
			e => e
				.JSRepl_Obj(0, EventDefs.Defs.CodeJSEventHandlerFunctions(eltId))
				.JSRepl_Val(1, eltId)
				.JSRepl_ArrOfObj(2, traces)
				.JSRepl_Obj(3, layout.PlotlySer())
				.JSRepl_Obj(4, config.PlotlySer())
		);

		return js;
	}
	*/


	public static void Update<T>(
		string eltId,
		(int, T)[] idxTraces,
		Layout layout
	) where T : ITrace
	{
		JS.Run(
			"""
			(async () => {
				await Plotly.update(
					____0____,
					____1____,
					____2____,
					____3____
				);
			})();
			""",
			e => e
				.JSRepl_Val(0, eltId)
				.JSRepl_Obj(1, PlotlyJson.PlotlySerNested(idxTraces.SelectA(f => f.Item2)))
				.JSRepl_Obj(2, layout.PlotlySer())
				.JSRepl_Arr(3, idxTraces.SelectA(f => f.Item1))
		);
	}



	static readonly JsonSerializerOptions jsonOpt = new()
	{
		Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
	};
	static string Ser<T>(this T obj) => JsonSerializer.Serialize(obj, jsonOpt);
	static T Deser<T>(this string str) => JsonSerializer.Deserialize<T>(str, jsonOpt)!;

	public static string GetJSCode(
		string eltId
	)
	{
		var allDataStr = JS.Return(
			"""
			(function() {
				const elt = document.getElementById(____0____);
				const { data, layout } = elt;
				return {
					data: data,
					layout: layout
				};
			})();
			""",
			e => e
				.JSRepl_Val(0, eltId)
		);

		var root = Deser<JsonObject>(allDataStr);
		var objData = (JsonArray)root["data"];
		var objLayout = (JsonObject)root["layout"];
		return JS.Fmt(
			"""
			import * as Plotly from 'plotly.js-dist-min';
			
			const data: Plotly.Data[] = ____0____;
			
			const layout: Partial<Plotly.Layout> = ____1____;
			
			// const errs = Plotly.validate(data, layout);
			// console.log(errs);
			
			const plot = await Plotly.newPlot(
				'app',
				data,
				layout
			);
			
			""",
			e => e
				.JSRepl_Obj(0, objData.Ser())
				.JSRepl_Obj(1, objLayout.Ser())
		);
	}
}