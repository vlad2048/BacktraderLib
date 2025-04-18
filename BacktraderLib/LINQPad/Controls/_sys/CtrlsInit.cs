using BacktraderLib._sys.JQuery;
using BacktraderLib._sys.Slider;
using BacktraderLib._sys.Tabulator;
using LINQPad;

namespace BacktraderLib._sys;

static class CtrlsInit
{
	public static void Init()
	{
		JQueryInit.Init();
		SliderInit.Init();
		TabulatorInit.Init();
		Ctrls.Init_Log();
		Ctrls.Init_ButtonScript();
		Ctrls.Init_ButtonCancellable();

		Util.HtmlHead.AddStyles("""
		.widget-horz {
			display: flex;
			column-gap: 5px;
		}
		.widget-horz-stretch {
			display: flex;
			column-gap: 5px;
			align-items: stretch;
		}
		.widget-vert {
			display: flex;
			flex-direction: column;
		}
		
		.widget {
			display: flex;
			align-items: baseline;
		}
		.widget > label {
			width: 100px;
			padding-right: 10px;
			text-overflow: ellipsis;
			overflow: hidden;
			white-space: nowrap;
			margin-top: 23px;
		}
		.widget-main {
			width: 300px;
		}
		""");
	}
}

static class CtrlsClasses
{
	public const string WidgetHorz = "widget-horz";
	public const string WidgetHorzStretch = "widget-horz-stretch";
	public const string WidgetVert = "widget-vert";

	public const string Widget = "widget";
	public const string WidgetMain = "widget-main";
}
