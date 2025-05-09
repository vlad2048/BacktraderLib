using BacktraderLib._sys.JQuery;
using BacktraderLib._sys.Slider;

namespace BacktraderLib._sys;



static class CtrlsClasses
{
	public const string Horz = "horz";
	public const string HorzStretch = "horz-stretch";
	public const string HorzCtrlRow = "horz-ctrlrow";
	public const string Vert = "vert";

	public const string Ctrl_Button_Cancel = "cancel";
	public const string Ctrl_Slider = "ctrl-slider";
	public const string Ctrl_Log = "ctrl-log";

	public const string TableWrapper = "table-wrapper";
	public const string TableControls = "table-controls";
}



static class CtrlsInit
{
	public static void Init()
	{
		JQueryInit.Init();
		SliderInit.Init();
		//TabulatorInit.Init();
		TableInit.Init();
		Ctrls.Init_Log();
		Css.Setup();
	}


	static class Css
	{
		public static void Setup()
		{
			General();
			HorzVert();
			InputSelect();
			Button();
			Label();
			Slider();
			Log();
			Tabulator();
		}


		static void General() => CssUtils.AddStyles(
			"""
			body {
				font-family: Consolas;
			}
			""");


		static void HorzVert() => CssUtils.AddStyles(
			"""
			.horz {
				display: flex;
			}
			.horz-stretch {
				display: flex;
				align-items: stretch;
				column-gap: 5px;
			}
			.horz-ctrlrow {
				display: flex;
				align-items: baseline;
				column-gap: 5px;
			}
			.vert {
				display: flex;
				flex-direction: column;
				row-gap: 30px;
			}
			""");
		

		static void InputSelect() => CssUtils.AddStyles(
			"""
			input, select {
				box-sizing: border-box;
				padding: 4px 10px;
				border: 1px solid #4b4b4b;
				border-radius: 5px;
				background: #1f1f1f;
				outline: none;
				margin: 0 5px 10px 5px;
			
				font-family: inherit;
				font-size: inherit;
				line-height: inherit;
				color: inherit;
				font: inherit;
			}
			""");


		static void Button() => CssUtils.AddStyles(
			"""
			button {
				margin: 0px 5px 0px 5px;
				padding: 5px 10px;
				border: 1px solid #25682a;
				background: linear-gradient(to bottom, #3FB449 0%, #25682a 100%);
				color: #fff;
				font-weight: bold;
				transition: color .3s, background .3s, opacity, .3s;
				cursor: pointer;
			}
			button:disabled {
				cursor: not-allowed;
				filter: alpha(opacity=65);
				opacity: 0.65;
				box-shadow: none;
			}
			button:hover {
				border: 1px solid #328e3a;
				background: linear-gradient(to bottom, #5fc768 0%, #328e3a 100%);
				color: #000;
			}
			button:focus {
				outline: rgb(16, 16, 16) auto 1px;
			}
			
			button.cancel {
			    border: 1px solid hsl(360, 48%, 28%);
			    background: linear-gradient(to bottom, hsl(360, 48%, 48%) 0%, hsl(360, 48%, 28%) 100%);
			}
			button.cancel:hover {
			    border: 1px solid hsl(360, 48%, 38%);
			    background: linear-gradient(to bottom, hsl(360, 48%, 58%) 0%, hsl(360, 48%, 38%) 100%);
			}
			
			""");


		static void Label() => CssUtils.AddStyles(
			"""
			label {
				display: flex;
				align-items: baseline;
			}
			label:has(input[type='checkbox']) {
				align-items: flex-end;
				column-gap: 5px;
			}
			""");


		static void Slider() => CssUtils.AddStyles(
			"""
			.ctrl-slider {
				width: 300px;
			}
			""");


		static void Log() => CssUtils.AddStyles(
			"""
			 .ctrl-log {
				overflow: auto;
				width: 100%;
				background-color: #121212;
			 }
			.ctrl-log > * {
				white-space: nowrap;
			}
			""");


		static void Tabulator() => CssUtils.AddStyles(
			"""
			.table-wrapper {
			}
			
			.table-controls {
				padding: 10px 5px 0px 5px;
				background: #121212;
				border: 1px solid #2d2c2c;
				border-bottom: 0px;
				font-size: 14px;
			}
			""");
	}
}
