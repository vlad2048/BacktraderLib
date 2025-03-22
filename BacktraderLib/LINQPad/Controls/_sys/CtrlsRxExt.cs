using LINQPad.Controls;

namespace BacktraderLib._sys;

static class CtrlsRxExt
{
	public static Label ToLabel<T>(this IObservable<T> source)
	{
		var label = new Label();
		source.Subscribe(e => label.Text = $"{e}").D(D);
		return label;
	}
}