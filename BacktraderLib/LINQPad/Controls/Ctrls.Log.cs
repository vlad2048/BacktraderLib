using LINQPad;
using RxLib;
using BaseUtils;
using BacktraderLib._sys;

namespace BacktraderLib;

public static partial class Ctrls
{
	static readonly TimeSpan DebounceTime = TimeSpan.FromMilliseconds(200);

	static Dictionary<string, IDisposable> dispMap = null!;

	internal static void Init_Log()
	{
		// ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
		dispMap?.D(D);
		dispMap = new Dictionary<string, IDisposable>();
	}
	
	public static (DumpContainer, Tag) Log()
	{
		var logDC = new DumpContainer();
		var logUI = new Tag("div")
		{
			Class = CtrlsClasses.Ctrl_Log,
			Kids =
			[
				logDC.ToTag().AddStyle([
				]),
			],
		};

		logDC.ContentAppended += _ =>
		{
			JS.Run(
				"""
				const elt = document.getElementById(____0____);
				if (!!elt)
					elt.scrollTop = elt.scrollHeight;
				""",
				e => e
					.JSRepl_Val(0, logUI.Id)
			);
		};


		void LogException(Exception ex, string origin)
		{
			logDC.AppendContent($"************ {origin} @ {Timestamp} isCancel:{ex.IsCancel()} ************");
			logDC.AppendContent(ex);
		}



		void ScheduleLogException(Exception ex, string origin)
		{
			var exName = ex.GetName();

			// JS.Run needs exceptions to detect wrong code
			if (ex.Message.Contains("JSRuntimeError")) return;

			// Playwright needs exceptions to detect is the Console is present
			if (ex.Message == "The handle is invalid." && ex.StackTrace?.Contains("System.ConsolePal.GetBufferInfo") == true) return;

			// LINQPad throws these for some reason
			if (exName == "RuntimeBinderException" && ex.Message.Contains("'LINQPad.Uncapsulator.Uncapsulator' does not contain a definition for")) return;
			if (exName == "RuntimeBinderException" && ex.Message.Contains("Cannot convert type 'LINQPad.Uncapsulator.Uncapsulator' to")) return;
			if (exName == "RuntimeBinderException" && ex.Message.Contains("Cannot implicitly convert type 'LINQPad.Uncapsulator.Uncapsulator' to")) return;

			if (exName == "TimeoutException") return;

			if (ex.IsCancel()) return;


			if (dispMap.ContainsKey(exName))
			{
				dispMap[exName].Dispose();
				dispMap.Remove(exName);
			}
			dispMap[exName] = Obs.Timer(DebounceTime).Subscribe(_ =>
			{
				LogException(ex, origin);
				dispMap[exName].Dispose();
				dispMap.Remove(exName);
			});
		}

		AppDomain.CurrentDomain.FirstChanceException += (_, evt) => ScheduleLogException(evt.Exception, "FirstChanceException");

		return (logDC, logUI);
	}

	static string GetName(this Exception ex) =>
		ex.IsCancel() switch
		{
			true => "Canceled",
			false => ex.GetType().Name,
		};

	static string Timestamp => $"{DateTime.Now:HH:mm:ss.ffffff}";
}