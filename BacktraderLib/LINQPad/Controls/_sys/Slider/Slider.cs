using BacktraderLib._sys.Utils;

namespace BacktraderLib._sys.Slider;


static class Slider
{
	public static Tag Single(
		double min,
		double max,
		double step,
		double from,
		string extraClasses,
		Action<int> onChange
	)
	{
		var id = IdGen.Make();
		var tag = new Tag("input")
		{
			Id = id,
			OnRenderJS = JS.Fmt(
				"""

				$('#____0____').ionRangeSlider({
					type: 'single',
					min: ____1____,
					max: ____2____,
					step: ____3____,
					from: ____4____,
					extra_classes: ____5____,
					onChange: function (data) {
						window.dispatch('____0____', [data.from, data.to]);
					},
				});

				""",
				e => e
					.JSRepl_Var(0, id)
					.JSRepl_Val(1, min)
					.JSRepl_Val(2, max)
					.JSRepl_Val(3, step)
					.JSRepl_Val(4, from)
					.JSRepl_Val(5, extraClasses)
			)
		};

		Events.ListenFast(id, () =>
		{
			var str = JS.Return(
				"""
				(function() {
					const elt = document.getElementById(____0____);
					return elt.value;
				})();
				""",
				e => e
					.JSRepl_Val(0, id)
			);

			var val = int.Parse(str);
			onChange(val);
		});

		return tag;
	}
	


	/*static int AsInt(this string args)
	{
		var arr = JsonConvert.DeserializeObject<int[]>(args)!;
		return arr[0];
	}*/
}
