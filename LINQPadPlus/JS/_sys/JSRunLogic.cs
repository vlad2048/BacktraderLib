using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using LINQPad;
using LINQPadPlus._sys.Structs;
using LINQPadPlus._sys.Utils;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LINQPadPlus._sys;

static class JSRunLogic
{
	public static void Run(
		[LanguageInjection(InjectedLanguage.JAVASCRIPT)] string code,
		CSharpSourceLocation csharpSourceLocation
	)
	{
		// @formatter:off
		var codeFull = $$"""
			try {

				{{code.JSIndent(1)}}

				'{{JSRunIdentifiers.ValidRunReturnValueIdentifier}}';
			} catch (err) {
				({
					id: '{{JSRunIdentifiers.RuntimeErrorIdentifier}}',
					message: err.message,
					stack: err.stack,
				});
			}
			""";
		// @formatter:on
		var ctx = new CSErrorCtx(false, code, codeFull, csharpSourceLocation);
		Return(ctx);
	}



	public static string Return(CSErrorCtx ctx)
	{
		var resObj = Util.InvokeScript(true, "eval", ctx.CodeFull);
		if (JSErrorUtils.TryGetReturnString(resObj, ctx, out var resStr))
			return resStr;
		var jsError = JSErrorFinder.Find(ctx, resObj);
		throw new JSRunException(jsError, ctx, resObj);
	}
}