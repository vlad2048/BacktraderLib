using BacktraderLib;
using BaseUtils;
using LINQPad;

namespace Feed.Final._sys.UtilsSteppers;

enum StepType
{
	Plus,
	Minus,
	Total,
}

sealed record Step(
	StepType Type,
	int Count,
	string Name,
	Action? View
);

class Stepper
{
	readonly List<Step> steps = [];

	public string Title { get; }
	public Step[] Steps => [..steps];

	public Stepper(string title) => Title = title;

	public Stepper Add(int count, string name, Action? view = null) => this.With(() => steps.Add(new Step(StepType.Plus, count, name, view)));
	public void Del(int count, string name, Action? view = null) => steps.Add(new Step(StepType.Minus, count, name, view));
	public Stepper Total(int count) => this.With(() => steps.Add(new Step(StepType.Total, count, "Total", null)));

	public object ToDump() => this.ToTag();
}

sealed record SideBySideSteppers(Stepper[] Steppers)
{
	public object ToDump() => new Tag("div")
	{
		Class = "stepper-sidebyside",
		Kids = Steppers.SelectA(e => e.ToTag()),
	};
}

sealed class Stepper<R>
{
	public R Result { get; }
	public SideBySideSteppers Steps { get; }
	public Stepper(R result, Stepper[] steps) => (Result, Steps) = (result, new SideBySideSteppers(steps));
}



static class StepperUtils
{
	public static Stepper<R> ToStepper<R>(this R result, params Stepper[] steps) => new(result, steps);

	public static T[] RemoveDups<T>(this IEnumerable<T> xs, Stepper stepper) => xs.RemoveDupsBy(e => e, stepper, "duplicates");
	
	public static T[] RemoveDupsBy<T, K>(this IEnumerable<T> xs, Func<T, K> key, Stepper stepper, string name)
	{
		var grps = xs
			.GroupBy(key)
			.SelectA(e => new Grp<K, T>(e.Key, e.ToArray()));

		var dups = grps
			.WhereA(g => g.Items.Length > 1);
		
		stepper.Del(dups.Sum(e => e.Items.Length), name, () => dups.Dump());

		return grps
			.Where(g => g.Items.Length == 1)
			.SelectManyA(e => e.Items);
	}

	sealed record Grp<K, V>(K Key, V[] Items);
}