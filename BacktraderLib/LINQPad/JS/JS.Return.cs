using System.Runtime.CompilerServices;
using BacktraderLib._sys;
using BacktraderLib._sys.Structs;
using BacktraderLib._sys.Utils;
using JetBrains.Annotations;

namespace BacktraderLib;

public static partial class JS
{
	public static string Return(
		[LanguageInjection(InjectedLanguage.JAVASCRIPT)] string code,
		Func<string, string>? replFun = null,
		[CallerMemberName] string? srcMember = null,
		[CallerFilePath] string? srcFile = null,
		[CallerLineNumber] int srcLine = 0
	)
	{
		code = Fmt(code, replFun);
		var codeFull = $$"""
			try {

				{{code.JSIndent(1)}}

			} catch (err) {
			({
				id: '{{JSRuntimeError.IdStr}}',
				message: err.message,
				stack: err.stack,
			});
			}
			""";

		var ctx = new CSErrorCtx(true, code, codeFull, srcMember, srcFile, srcLine);
		return JSRunLogic.Return(ctx);
	}
}