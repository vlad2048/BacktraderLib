using System.Runtime.CompilerServices;
using BacktraderLib._sys;
using BacktraderLib._sys.Structs;
using BacktraderLib._sys.Utils;
using JetBrains.Annotations;
using LINQPad;

namespace BacktraderLib;

public static class JS
{
	static bool ErrorDumpingDisabled { get; set; }


	internal static void Init() => JSInit.Init();

	
	public static void DisableErrorDumping() => ErrorDumpingDisabled = true;


	public static void Run(
		[LanguageInjection(InjectedLanguage.JAVASCRIPT)]
		string code,
		Func<string, string> replFun,
		[CallerMemberName] string? srcMember = null,
		[CallerFilePath] string? srcFile = null,
		[CallerLineNumber] int srcLine = 0
	) =>
	// ReSharper disable ExplicitCallerInfoArgument
		Run(Fmt(code, replFun), srcMember, srcFile, srcLine);
	// ReSharper restore ExplicitCallerInfoArgument


	public static void Run(
		[LanguageInjection(InjectedLanguage.JAVASCRIPT)] string code,
		[CallerMemberName] string? srcMember = null,
		[CallerFilePath] string? srcFile = null,
		[CallerLineNumber] int srcLine = 0
	)
	{
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
		var ctx = new CSErrorCtx(false, code, codeFull, srcMember, srcFile, srcLine);
		RunInternal(ctx);
	}



	public static string Return(
		[LanguageInjection(InjectedLanguage.JAVASCRIPT)] string code,
		Func<string, string> replFun,
		[CallerMemberName] string? srcMember = null,
		[CallerFilePath] string? srcFile = null,
		[CallerLineNumber] int srcLine = 0
	) =>
	// ReSharper disable ExplicitCallerInfoArgument
		Return(Fmt(code, replFun), srcMember, srcFile, srcLine);
	// ReSharper restore ExplicitCallerInfoArgument

	public static string Return(
		[LanguageInjection(InjectedLanguage.JAVASCRIPT)] string code,
		[CallerMemberName] string? srcMember = null,
		[CallerFilePath] string? srcFile = null,
		[CallerLineNumber] int srcLine = 0
	)
	{
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
		return RunInternal(ctx);
	}



	public static string Fmt(
		[LanguageInjection(InjectedLanguage.JAVASCRIPT)]
		string code,
		Func<string, string> replFun
	)
		=> replFun(code);



	public static string Fmt(
		[LanguageInjection(InjectedLanguage.JAVASCRIPT)]
		string code
	)
		=> code;





	static string RunInternal(CSErrorCtx ctx)
	{
		var resObj = Util.InvokeScript(true, "eval", ctx.CodeFull);
		if (JSErrorUtils.TryGetReturnString(resObj, ctx, out var resStr))
			return resStr;
		var jsError = JSErrorUtils.GetError(ctx, resObj);
		if (!ErrorDumpingDisabled)
		{
			var fullDescription = jsError.GetFullDescription(resObj);
			Util.FixedFont(fullDescription, "#f53fee").Dump();
		}

		throw jsError.Exception;
	}
}