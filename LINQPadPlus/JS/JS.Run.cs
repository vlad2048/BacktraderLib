using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using LINQPadPlus._sys;

namespace LINQPadPlus;

public static partial class JS
{
	public static void Run(
		[LanguageInjection(InjectedLanguage.JAVASCRIPT)]
		string code,
		Func<string, string>? replFun = null,
		[CallerMemberName] string? srcMember = null,
		[CallerFilePath] string? srcFile = null,
		[CallerLineNumber] int srcLine = 0
	) =>
		JSRunLogic.Run(
			Fmt(code, replFun),
			new CSharpSourceLocation(srcMember, srcFile, srcLine)
		);
}