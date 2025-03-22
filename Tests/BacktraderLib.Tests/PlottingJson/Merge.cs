using BacktraderLib._sys;
using Shouldly;

namespace BacktraderLib.Tests.PlottingJson;

class Merge
{
	sealed record Foo
	{
		public double Skip => 123.456;
		public int[]? X { get; init; }
		public int[]? Y { get; init; }
		public string? Name { get; init; }
	}
	
	
	[Test]
	public void Simple() =>
		ObjectMerger.Merge(
			new Foo
			{
				X = [1, 2],
				Y = [3, 4],
				Name = null,
			},
			new Foo
			{
				X = null,
				Y = [5, 6],
				Name = "Name",
			}
		).Check(
			new Foo
			{
				X = [1, 2],
				Y = [5, 6],
				Name = "Name",
			}
		);
}


file static class TestUtils
{
	public static void Check<T>(this T objAct, T objExp) => objAct.PlotlySer().ShouldBe(objExp.PlotlySer());
}