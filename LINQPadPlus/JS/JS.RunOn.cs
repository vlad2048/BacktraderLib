using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace LINQPadPlus;

public static partial class JS
{
	/// <summary>
	/// JS.RunOn(IdBar, "elt =&gt; elt.style.display = 'none'");
	/// </summary>
	public static void RunOn(
		string id,
		[LanguageInjection(InjectedLanguage.JAVASCRIPT)] string codeFun,
		Func<string, string>? replFun = null,
		[CallerMemberName] string? srcMember = null,
		[CallerFilePath] string? srcFile = null,
		[CallerLineNumber] int srcLine = 0
	) =>
		Run(
			"""
			
			(async () => {
				const elt = await window.waitForElement(____998____);
				(____999____)(elt);
			})();
			
			""",
			e =>
				Fmt(
					e
						.JSRepl_Val(998, id)
						.JSRepl_Var(999, codeFun),
					replFun
				),
			srcMember,
			// ReSharper disable ExplicitCallerInfoArgument
			srcFile,
			srcLine
			// ReSharper restore ExplicitCallerInfoArgument
		);
}
/*

const elt = document.getElementById(____998____);
(____999____)(elt);

*/