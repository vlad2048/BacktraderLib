using BacktraderLib._sys;
using JetBrains.Annotations;
using System.Runtime.CompilerServices;

namespace BacktraderLib;

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
		// ReSharper disable ExplicitCallerInfoArgument
		JSRunLogic.Run(Fmt(code, replFun), srcMember, srcFile, srcLine);
	// ReSharper restore ExplicitCallerInfoArgument
}