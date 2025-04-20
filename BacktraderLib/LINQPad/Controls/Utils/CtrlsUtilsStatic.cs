using BacktraderLib._sys;

namespace BacktraderLib;

public static class CtrlsUtilsStatic
{
	public static Tag horz(Tag[] kids) => kids.Group(CtrlsClasses.Horz);
	public static Tag horzStretch(Tag[] kids) => kids.Group(CtrlsClasses.HorzStretch);
	public static Tag horzCtrlRow(Tag[] kids) => kids.Group(CtrlsClasses.HorzCtrlRow);
	public static Tag vert(Tag[] kids) => kids.Group(CtrlsClasses.Vert);

	static Tag Group(this Tag[] kids, string cls) => new("div")
	{
		Kids = kids,
		Class = cls,
	};


	/*
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
	*/
}