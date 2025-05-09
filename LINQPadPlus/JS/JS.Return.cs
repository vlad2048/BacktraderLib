using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using LINQPadPlus._sys;
using LINQPadPlus._sys.Structs;
using LINQPadPlus._sys.Utils;

namespace LINQPadPlus;

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
					id: '{{JSRunIdentifiers.RuntimeErrorIdentifier}}',
					message: err.message,
					stack: err.stack,
				});
			}
			""";

		var ctx = new CSErrorCtx(true, code, codeFull, new CSharpSourceLocation(srcMember, srcFile, srcLine));
		return JSRunLogic.Return(ctx);
	}
}