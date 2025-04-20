using BacktraderLib._sys;

namespace BacktraderLib;

public enum ButtonStyle
{
	Main,
	Cancel,
}


public static partial class Ctrls
{
	public static Tag Button(
		string name,
		Action action,
		ButtonStyle style = ButtonStyle.Main,
		IObservable<bool>? ΔenableOn = null
	) =>
		new Tag("button", null, name)
			{
				OnClick = () =>
				{
					try
					{
						action();
					}
					catch (Exception)
					{
					}
				},
				Class = style switch
				{
					ButtonStyle.Main => null,
					ButtonStyle.Cancel => CtrlsClasses.Ctrl_Button_Cancel,
					_ => throw new ArgumentException($"Unknown ButtonStyle: {style}"),
				},
			}
			.Dyna(ΔenableOn, (enableOn, t) => t.SetEnabled(enableOn));

	/*public static Tag Button(
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
		.Dyna(ΔenableOn, (enableOn, t) => t.SetEnabled(enableOn));*/
}