using System.Diagnostics;
using BacktraderLib._sys.JQuery;
using BacktraderLib._sys.Slider;
using BacktraderLib._sys.Tabulator;
using LINQPad;

namespace BacktraderLib._sys;



static class CtrlsClasses
{
	/*public const string WidgetHorz = "widget-horz";
	public const string WidgetHorzStretch = "widget-horz-stretch";
	public const string WidgetVert = "widget-vert";

	public const string Widget = "widget";
	public const string WidgetMain = "widget-main";*/

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
		TabulatorInit.Init();
		Ctrls.Init_Log();
		Css();
	}

	static void Css()
	{
		Util.HtmlHead.AddStyles(
			"""
			/***************
			 * horz / vert *
			 ***************/
			.horz {
				display: flex;
			}
			.horz-stretch {
				display: flex;
				align-items: stretch;
			}
			.horz-ctrlrow {
				display: flex;
				align-items: baseline;
				column-gap: 5px;
			}
			.vert {
				display: flex;
				flex-direction: column;
			}
			
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
			
			
			/********
			* label *
			*********/
			label {
				display: flex;
				align-items: baseline;
			}
			label:has(input[type='checkbox']) {
				align-items: flex-end;
				column-gap: 5px;
			}
			
			
			
			/*********
			* button *
			**********/
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
			
			
			
			/*********
			* slider *
			**********/
			.ctrl-slider {
				width: 300px;
			}
			
			
			
			/*******
			 * Log *
			 *******/
			 .ctrl-log {
				overflow: auto;
				width: 100%;
				background-color: #121212;
			 }
			.ctrl-log > * {
				white-space: nowrap;
			}
			
			
			
			/************
			* tabulator *
			*************/
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

	static void CssOld()
	{
		Util.HtmlHead.AddStyles(
			"""
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



			/*************
			 * Tabulator *
			 *************/
			.table-wrapper {
			}

			.table-controls {
				display: flex;
				padding: 10px 5px 10px 5px;
				background: #121212;
				border: 1px solid #2d2c2c;
				border-bottom: 0px;
				font-size: 14px;
			}

			.table-controls input, .table-controls select {
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

			.table-controls button {
				margin: 0px 5px 0px 5px;
				padding: 5px 10px;
				border: 1px solid #25682a;
				background: linear-gradient(to bottom, #3FB449 0%, #25682a 100%);
				color: #fff;
				font-weight: bold;
				transition: color .3s, background .3s, opacity, .3s;
				cursor: pointer;
			}
			.table-controls button:disabled {
				cursor: not-allowed;
				filter: alpha(opacity=65);
				opacity: 0.65;
				box-shadow: none;
			}
			.table-controls button:hover {
				border: 1px solid #328e3a;
				background: linear-gradient(to bottom, #5fc768 0%, #328e3a 100%);
				color: #000;
			}
			.table-controls button:focus {
				outline: rgb(16, 16, 16) auto 1px;
			}



			/*********************
			 * ButtonCancellable *
			 *********************/
			.table-controls button.button-cancel {
			    border: 1px solid hsl(360, 48%, 28%);
			    background: linear-gradient(to bottom, hsl(360, 48%, 48%) 0%, hsl(360, 48%, 28%) 100%);
			}
			.table-controls button.button-cancel:hover {
			    border: 1px solid hsl(360, 48%, 38%);
			    background: linear-gradient(to bottom, hsl(360, 48%, 58%) 0%, hsl(360, 48%, 38%) 100%);
			}



			/*******
			 * Log *
			 *******/
			.log > * {
				white-space: nowrap;
			}

			""");
	}
}
