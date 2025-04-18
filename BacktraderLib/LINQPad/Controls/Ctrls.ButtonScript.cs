using LINQPad;
using RxLib;
using ScrapeUtils;

namespace BacktraderLib;

public static partial class Ctrls
{
	static int runCount;

	public static Tag ButtonScript(
		IScriptSource source,
		Web web
	)
	{
		var ΔisRunning = Var.Make(false);
		ΔisRunning.Skip(1).Subscribe(e => web.Log.Log($"ΔisRunning <- {e}")).D(D);

		CancellationTokenSource? cancelSource = null;

		CancellationToken InitCancel()
		{
			web.Log.Log("InitCancel()");
			cancelSource?.Dispose();
			cancelSource = new CancellationTokenSource();
			return cancelSource.Token;
		}

		void DoCancel()
		{
			web.Log.Log("DoCancel()");
			cancelSource?.Cancel();
			cancelSource = null;
		}

		void FinishCancel()
		{
			web.Log.Log("FinishCancel()");
			cancelSource?.Dispose();
			cancelSource = null;
		}

		return new Tag("button", null, "Script")
		{
			// ReSharper disable once AsyncVoidLambda
			OnClick = async () =>
			{
				if (!ΔisRunning.V)
				{
					web.Log.UpdateContent(Util.RawHtml($"<h2>Script run {runCount++}</h2>"));
					var cancelToken = InitCancel();
					using var _ = web.UseCancelToken(cancelToken);
					ΔisRunning.V = true;
					await ScriptWatcher.Run(source, web);
					ΔisRunning.V = false;
					FinishCancel();
				}
				else
				{
					DoCancel();
					ΔisRunning.V = false;
				}
			},
		}
		.Dyna(ΔisRunning, (v, t) => t
			.SetText(v ? "Cancel" : "Script")
			.SetClass(v ? "" : "script-off"));
	}



	static void Log(this DumpContainer logDC, string str) => logDC.AppendContent($"[ScriptRunner - Log] {str}");


	internal static void Init_ButtonScript() => Util.HtmlHead.AddStyles(
		"""
		:root {
		    --color-button-off-hue: 360;
		}
		
		
		.table-controls button.script-off {
		    border: 1px solid hsl(var(--color-button-off-hue), 48%, 28%);
		    background: linear-gradient(to bottom, hsl(var(--color-button-off-hue), 48%, 48%) 0%, hsl(var(--color-button-off-hue), 48%, 28%) 100%);
		}
		.table-controls button.script-off:hover {
		    border: 1px solid hsl(var(--color-button-off-hue), 48%, 38%);
		    background: linear-gradient(to bottom, hsl(var(--color-button-off-hue), 48%, 58%) 0%, hsl(var(--color-button-off-hue), 48%, 38%) 100%);
		}
		
		""");
}