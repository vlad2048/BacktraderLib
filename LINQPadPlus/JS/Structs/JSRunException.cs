using LINQPad;
using LINQPadPlus._sys.Structs;
using LINQPadPlus._sys.Utils;

namespace LINQPadPlus;

public sealed class JSRunException : Exception
{
	public string WhenCalling { get; }
	public CSharpSourceLocation CSharpSourceLocation { get; }
	public string? Result { get; }
	public string? JavaScriptStackTrace { get; }
	public object? JavaScriptCode { get; }
	

	internal JSRunException(IJSError error, CSErrorCtx ctx, object? resObj)
		: base(error.GetMessage(ctx.IsReturn), error.GetInnerException())
	{
		WhenCalling = ctx.IsReturn switch
		{
			false => "JS.Run()",
			true => "JS.Return()",
		};

		CSharpSourceLocation = ctx.CSharpSourceLocation;
		
		Result = resObj switch
		{
			null => "_",
			string e => $"'{e}'",
			_ => $"[{resObj.GetType().Name}]",
		};
		
		
		JavaScriptStackTrace = error switch
		{
			JSRuntimeError err => err.Stack.JSIndent(3),
			_ => null,
		};
		
		JavaScriptCode = ctx.CodeFull switch
		{
			not null => Util.SyntaxColorText(ctx.CodeFull, SyntaxLanguageStyle.JavaScript),
			null => null,
		};
	}




	
}



file static class JSRunExceptionUtils
{
	public static string GetMessage(this IJSError error, bool isReturn)
	{
		var funName = isReturn switch
		{
			false => "JS.Run()",
			true => "JS.Return()",
		};
		var msg = error switch
		{
			JSCompilationError err => $"Compilation error - {err.Message}",
			JSRuntimeError err => $"Runtime error - {err.Message}",
			JSWrongReturnTypeError err => $"Wrong return type - {err.Message}",
			JSUnknownError => "Unknown error - This should be impossible!",
			_ => "Invalid error type - This should be impossible!",
		};
		return $"{funName}: {msg}";
	}


	public static Exception? GetInnerException(this IJSError error) =>
		error switch
		{
			JSCompilationError err => err.InnerException,
			_ => null,
		};
}