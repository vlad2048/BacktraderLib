using BacktraderLib._sys;

namespace BacktraderLib;

public static class CtrlsUtilsStatic
{
	public static Tag horz(Tag[] kids) => new("div")
	{
		Class = CtrlsClasses.WidgetHorz,
		Kids = kids,
	};
	public static Tag horzStretch(Tag[] kids) => new("div")
	{
		Class = CtrlsClasses.WidgetHorzStretch,
		Kids = kids,
	};
	public static Tag vert(Tag[] kids) => new("div")
	{
		Class = CtrlsClasses.WidgetVert,
		Kids = kids,
	};
}