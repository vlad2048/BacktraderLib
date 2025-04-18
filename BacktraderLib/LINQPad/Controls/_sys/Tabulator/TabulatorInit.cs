using LINQPad;

namespace BacktraderLib._sys.Tabulator;

static class TabulatorInit
{
	const string ResourceFolder = "BacktraderLib.LINQPad.Controls._sys.Tabulator";

	public static void Init()
	{
		Util.HtmlHead.AddStyles(ResourceLoader.Load($"{ResourceFolder}.tabulator_site_dark.min.css"));
		Util.HtmlHead.AddScript(ResourceLoader.Load($"{ResourceFolder}.tabulator.min.js"));

		Util.HtmlHead.AddStyles(
			"""
			
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
			
			""");
	}


	public static class Classes
	{
		public const string TableWrapper = "table-wrapper";
		public const string TableControls = "table-controls";
	}
}
