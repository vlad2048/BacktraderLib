using BacktraderLib._sys;
using BacktraderLib._sys.Slider;
using RxLib;

namespace BacktraderLib;

public static partial class Ctrls
{
	public static (IRoVar<int>, Tag) IntSlider(
		string name,
		int init,
		int min,
		int max
	)
	{
		var Δrx = Var.Make(init);
		var ui = Slider.Single(
			min,
			max,
			1,
			init,
			CtrlsClasses.WidgetMain,
			e => Δrx.V = e
		);
		return (Δrx, ui.WithLabel(name));
	}


	static Tag WithLabel(this Tag tag, string? name) =>
		name switch
		{
			not null =>
				new("div")
				{
					Class = CtrlsClasses.Widget,
					Kids =
					[
						new Tag("label", null, name),
						tag,
					],
				},
			null => tag,
		};




	/*public static (IRoVar<int>, object) IntSliderNative(string name, int init, int min, int max)
	{
		var rx = Var.Make(init, D);

		var slider = new RangeControl(min, max, init);
		slider.HtmlElement.AddEventListener("input", (_, _) => rx.V = slider.Value);
		slider.Styles["width"] = "900px";

		return (
			rx,
			slider
		);
	}*/
}