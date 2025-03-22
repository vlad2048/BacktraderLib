using BacktraderLib._sys;
using JetBrains.Annotations;
using Shouldly;

namespace BacktraderLib.Tests.PlottingJson;

class SerializationNested
{
	sealed record Foo(
		string? Name,
		int[] Arr
	);

	[Test]
	public void Simple() =>
		PlotlyJson.PlotlySerNested([
				new Foo(
					null,
					[1, 2, 3]
				),
				new Foo(
					"Name",
					[4, 5]
				),
			])
			.Check(
				"""
				{
					"arr": [
						[
							1,
							2,
							3
						],
						[
							4,
							5
						]
					],
					"name": [
						null,
						"Name"
					]
				}
				"""
			);
}


file static class TestUtils
{
	public static void Check(this string strAct, [LanguageInjection(InjectedLanguage.JSON)] string strExp) => strAct.ShouldBe(strExp);
}