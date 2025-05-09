using System.Text;
using BacktraderLib._sys.Utils;
using Microsoft.ClearScript;
using Microsoft.ClearScript.V8;

namespace BacktraderLib._sys.Structs;

interface IJSError
{
	JSErrorType Type { get; }
	string ExceptionMessage { get; }
	Exception? InnerException { get; }
}

sealed record JSCompilationError(string Message, Exception? InnerException) : IJSError
{
	static readonly V8ScriptEngine engine = new();

	public JSErrorType Type => JSErrorType.Compilation;
	public string ExceptionMessage => $"{Type} Error: {Message}";

	public static JSCompilationError? TryGet(string code)
	{
		try
		{
			engine.Compile(code);
			return null;
		}
		catch (ScriptEngineException ex)
		{
			return new JSCompilationError(ex.ErrorDetails, ex.InnerException);
		}
	}
}

sealed record JSRuntimeError(string Id, string Message, string Stack) : IJSError
{
	public JSErrorType Type => JSErrorType.Runtime;
	public string ExceptionMessage => $"{Type} Error: {Message}";
	public Exception? InnerException => null;
	public const string IdStr = "JavaScriptError";

	public static JSRuntimeError? TryGet(object? resObj)
	{
		if (resObj is not string str) return null;
		try
		{
			var err = JsonUtils.Deser<JSRuntimeError>(str);
			if (err.Id != IdStr) return null;
			return err;
		}
		catch (Exception)
		{
			return null;
		}
	}
}


sealed record JSWrongReturnType(string Message) : IJSError
{
	public JSErrorType Type => JSErrorType.WrongReturnType;
	public string ExceptionMessage => $"{Type} Error: {Message}";
	public Exception? InnerException => null;

	public static JSWrongReturnType? TryGet(bool isReturn, object? resObj)
	{
		if (resObj == null)
			return new JSWrongReturnType("Result is null");

		if (resObj is not string resStr)
			return new JSWrongReturnType($"Result is not a string (but a {resObj.GetType().Name})");

		if (!isReturn && resStr != "ok")
			return new JSWrongReturnType($"Result is not the string 'ok' as it should be for JS.Run() (but '{resStr}')");

		if (isReturn && resStr == "{}")
			return new JSWrongReturnType("Result is an empty object ('{}') string. Did you forget to invoke a function expression?");

		return null;
	}
}

sealed record JSUnknownError : IJSError
{
	public JSErrorType Type => JSErrorType.Unknown;
	public string ExceptionMessage => $"{Type} Error";
	public Exception? InnerException => null;
}



sealed record JSErrorNfo(
	CSErrorCtx Ctx,
	IJSError Error
)
{
	public JSRunException Exception => new(Error.Type, Error.ExceptionMessage, Error.InnerException);

	public string GetFullDescription(object? resObj)
	{
		var sb = new StringBuilder();

		var code = Error.Type switch
		{
			JSErrorType.Runtime or JSErrorType.Unknown => Ctx.CodeFull,
			_ => Ctx.Code,
		};
		var resStr = resObj switch
		{
			null => "_",
			string e => $"'{e}'",
			_ => $"[{resObj.GetType().Name}]",
		};

		sb.AddBig($"JavaScript {Error.Type} error in {(Ctx.IsReturn ? "JS.Return()" : "JS.Run()")}");
		sb.AddLine($"""
			CallerMemberName: {Ctx.SrcMember}
			CallerFilePath  : {Ctx.SrcFile}
			CallerLineNumber: {Ctx.SrcLine}
			
			Result          : {resStr}
			
			--------- Code ---------
				{code.JSIndent(2)}
			------------------------
		""");
		sb.AddNewLine();

		switch (Error)
		{
			case JSCompilationError err:
				sb.AddSmall("Compilation Error:");
				sb.AddLine($"""
						Message: {err.Message}
				""");
				break;

			case JSRuntimeError err:
				sb.AddSmall("Runtime Error:");
				sb.AddLine($"""
						Message: {err.Message}
						Stack  :
							{err.Stack.JSIndent(3)}
				""");
				break;

			case JSWrongReturnType err:
				sb.AddSmall("WrongReturnType Error:");
				sb.AddLine($"""
						Message: {err.Message}
				""");
				break;

			case JSUnknownError:
				sb.AddSmall("Unknown Error:");
				sb.AddLine("""
						Message: This should be impossible!
				""");
				break;

			default:
				throw new ArgumentException($"Invalid Error type: '{Error.GetType().Name}'");
		}

		return sb.ToString();
	}
}



file static class StringBuilderFileExt
{
	public static void AddNewLine(this StringBuilder sb) => sb.AppendLine();
	public static void AddLine(this StringBuilder sb, string s) => sb.AppendLine(s);
	public static void AddBig(this StringBuilder sb, string title)
	{
		sb.AddLine(title);
		sb.AddLine(new string('=', title.Length));
	}
	public static void AddSmall(this StringBuilder sb, string title)
	{
		var pad = "\t";
		sb.AddLine(pad + title);
		sb.AddLine(pad + new string('-', title.Length));
	}
}