using BacktraderLib._sys;
using JetBrains.Annotations;
using Shouldly;

namespace BacktraderLib.Tests.PlottingJson;

class Serialization
{
	[Test] public void ScatterMode_1() => ScatterMode.None.SerCheckStr("none");
	[Test] public void ScatterMode_2() => ScatterMode.LinesMarkers.SerCheckStr("lines+markers");
	[Test] public void ScatterMode_3() => ScatterMode.LinesMarkersText.SerCheckStr("lines+markers+text");

	[Test] public void TraceType_1() => TraceType.Bar.SerCheckStr("bar");
	[Test] public void TraceType_2() => TraceType.Histogram2DContour.SerCheckStr("histogram2dcontour");
	[Test] public void TraceType_3() => TraceType.ScatterPolarGL.SerCheckStr("scatterpolargl");

	[Test]
	public void LayoutTemplate_1() =>
		new Layout
		{
			Height = 123,
		}.SerCheck(
		// @formatter:off
		"""
		{
			"height": 123
		}
		""");
	// @formatter:on

	[Test]
	public void LayoutTemplate_2() =>
		new Layout
		{
			Height = 456,
			Template =
				// @formatter:off
				"""
				{
					"name": "val",
					"inner": { "arr": [3, 5, 7] }
				}
				""",

			// @formatter:on
		}.SerCheck(
		// @formatter:off
		"""
		{
			"height": 456,
			"template": {
				"name": "val",
				"inner": {
					"arr": [
						3,
						5,
						7
					]
				}
			}
		}
		""");
	// @formatter:on
}


file static class TestUtils
{
	public static void SerCheckStr<T>(this T objAct, string strExp) => objAct.SerCheck($"\"{strExp}\"");
	public static void SerCheck<T>(this T objAct, [LanguageInjection(InjectedLanguage.JSON)] string strExp) => objAct.PlotlySer().ShouldBe(strExp);
}