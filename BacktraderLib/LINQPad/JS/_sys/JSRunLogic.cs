using System.Runtime.CompilerServices;
using BacktraderLib._sys.Structs;
using BacktraderLib._sys.Utils;
using JetBrains.Annotations;
using LINQPad;

namespace BacktraderLib._sys;

static class JSRunLogic
{
	public static void Run(
		[LanguageInjection(InjectedLanguage.JAVASCRIPT)] string code,
		[CallerMemberName] string? srcMember = null,
		[CallerFilePath] string? srcFile = null,
		[CallerLineNumber] int srcLine = 0
	)
	{
		// @formatter:off
		var codeFull = $$"""
			try {

				{{code.JSIndent(1)}}

				'ok';
			} catch (err) {
			({
				id: '{{JSRuntimeError.IdStr}}',
				message: err.message,
				stack: err.stack,
			});
			}
			""";
		// @formatter:on
		var ctx = new CSErrorCtx(false, code, codeFull, srcMember, srcFile, srcLine);
		Return(ctx);
	}



	public static string Return(CSErrorCtx ctx)
	{
		var resObj = Util.InvokeScript(true, "eval", ctx.CodeFull);
		if (JSErrorUtils.TryGetReturnString(resObj, ctx, out var resStr))
			return resStr;
		var jsError = JSErrorUtils.GetError(ctx, resObj);
		if (!JS.ErrorDumpingDisabled)
		{
			var fullDescription = jsError.GetFullDescription(resObj);
			Util.FixedFont(fullDescription, "#f53fee").Dump();
		}

		throw jsError.Exception;
	}
}