namespace BacktraderLib;

public static partial class Ctrls
{
	public static Tag Button(
		string name,
		Func<Task> action,
		IObservable<bool>? ΔenableOn = null
	) =>
		new Tag("button", null, name)
		{
			OnClick = async () =>
			{
				try
				{
					await action();
				}
				catch (Exception)
				{
				}
			},
		}
		.Dyna(ΔenableOn, (enableOn, t) => t.SetEnabled(enableOn));
}