using BacktraderLib._sys;

namespace BacktraderLib;

public static class CtrlsUtilsStatic
{
	public static Tag horz(Tag[] kids) => new("div")
	{
		Class = CtrlsClasses.WidgetHorz,
		Kids = kids,
	};
	public static Tag vert(Tag[] kids) => new("div")
	{
		Class = CtrlsClasses.WidgetVert,
		Kids = kids,
	};


	/*
	public static object horz(params object[] arr) => Util.HorizontalRun(true, arr);
	public static object vert(params object[] arr) => Util.VerticalRun(arr);

	public static Control WidgetHorz(params Control[] arr) =>
		new("div", arr)
		{
			CssClass = CtrlsClasses.WidgetHorz,
		};
	public static Control WidgetVert(params Control[] arr) =>
		new("div", arr)
		{
			CssClass = CtrlsClasses.WidgetVert,
		};
	*/



	//public static object horz(params Control[] arr) => Util.HorizontalRun(true, arr.OfType<object>().ToArray());
	//public static object vert(params Control[] arr) => Util.VerticalRun(arr.OfType<object>().ToArray());

	/*public static Div horz(params Control[] ctrls) =>
		new Div(ctrls)
			.With([
				"display: flex",
				"align-items: center",
			]);

	public static Div vert(params Control[] ctrls) =>
		new Div(ctrls)
			.With([
				"display: flex",
				"flex-direction: column",
			]);*/
}