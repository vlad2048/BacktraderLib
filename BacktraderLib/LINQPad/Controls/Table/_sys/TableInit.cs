using System.Text.Json.Nodes;
using BacktraderLib._sys.Utils;
using BacktraderLib.Utils;
using LINQPad;

namespace BacktraderLib._sys;

static class TableInit
{
	const string ResourceFolder = "BacktraderLib.LINQPad.Controls.Table._sys";

	public static void Init()
	{
		Util.HtmlHead.AddStyles(ResourceLoader.Load($"{ResourceFolder}.tabulator_site_dark.min.css"));
		Util.HtmlHead.AddScript(ResourceLoader.Load($"{ResourceFolder}.tabulator.min.js"));
		JS.Run(
			"""
			
			const pageSize = 30;
			
			function changeSel(table, fun, cnt) {
			    if (cnt === undefined) cnt = 1;
			    const xs = table.getSelectedRows();
			    if (xs.length !== 1) return;
			    const rowPrev = xs[0];
			
			    let rowCur = rowPrev;
			    let i = 0;
			    while (i < cnt) {
			        const rowNext = fun(rowCur);
			        if (!rowNext) break;
			        rowCur = rowNext;
			        i++;
			    }
			    if (rowCur !== rowPrev) {
			        table.deselectRow(rowPrev.getIndex());
			        table.selectRow(rowCur.getIndex());
			        table.scrollToRow(rowCur, "top", false);
			    }
			}
			
			Tabulator.extendModule("keybindings", "actions", {
				"moveSelUp": function() { changeSel(this.table, r => r.getPrevRow()); },
				"moveSelDown": function() { changeSel(this.table, r => r.getNextRow()); },
				
				"moveSelPgUp": function() { changeSel(this.table, r => r.getPrevRow(), pageSize); },
				"moveSelPgDown": function() { changeSel(this.table, r => r.getNextRow(), pageSize); },
				
				"moveSelHome": function() { changeSel(this.table, r => this.table.getRows('visible').at(0)); },
				"moveSelEnd": function() { changeSel(this.table, r => this.table.getRows('visible').at(-1)); },
			});
			
			window.addEventListener("keydown", function(e) {
			    const target = e.target;
			    if (!target) return;
			    if (target.className !== "tabulator-tableholder") return;
			    if([38, 40, 33, 34, 36, 35].indexOf(e.keyCode) > -1) {
			        e.preventDefault();
			    }
			}, false);
			
			"""
		);
	}

	public static readonly JsonObject jsonKeybindings =
		new
			{
				keybindings = new
				{
					moveSelUp = 38,
					moveSelDown = 40,

					moveSelPgUp = 33,
					moveSelPgDown = 34,

					moveSelHome = 36,
					moveSelEnd = 35,
				}
			}
			.ToJsonObjectGen();
}
