using LINQPadPlus._sys.Structs;
using Microsoft.ClearScript;
using Microsoft.ClearScript.V8;

namespace LINQPadPlus._sys.Utils;

static class JSErrorFinder
{
	static readonly V8ScriptEngine engine = new();


	public static IJSError Find(CSErrorCtx ctx, object? resObj) =>
		CheckRuntimeError(resObj) ??
		CheckCompilationError(ctx.Code) ??
		CheckWrongReturnTypeError(ctx.IsReturn, resObj) ??
		new JSUnknownError();



	public static IJSError? CheckRuntimeError(object? resObj)
	{
		if (resObj is not string str) return null;
		try
		{
			var err = JsonUtils.Deser<JSRuntimeError>(str);
			return (err.Id != JSRunIdentifiers.RuntimeErrorIdentifier) switch
			{
				true => null,
				false => err,
			};
		}
		catch (Exception)
		{
			return null;
		}
	}
	

	static IJSError? CheckCompilationError(string code)
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


	static IJSError? CheckWrongReturnTypeError(bool isReturn, object? resObj)
	{
		if (resObj == null)
			return new JSWrongReturnTypeError("Result is null");

		if (resObj is not string resStr)
			return new JSWrongReturnTypeError($"Result is not a string (but a {resObj.GetType().Name})");

		if (!isReturn && resStr != JSRunIdentifiers.ValidRunReturnValueIdentifier)
			return new JSWrongReturnTypeError($"Result is not the string '{JSRunIdentifiers.ValidRunReturnValueIdentifier}' as it should be for JS.Run() (but '{resStr}')");

		if (isReturn && resStr == "{}")
			return new JSWrongReturnTypeError("Result is an empty object ('{}') string. Did you forget to invoke a function expression?");

		return null;
	}
}