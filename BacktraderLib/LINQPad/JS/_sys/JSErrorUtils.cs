using System.Diagnostics.CodeAnalysis;
using BacktraderLib._sys.Structs;

namespace BacktraderLib._sys;

static class JSErrorUtils
{
	public static bool TryGetReturnString(object? resObj, CSErrorCtx ctx, [NotNullWhen(true)] out string? resStr)
	{
		resStr = null;
		switch (ctx.IsReturn)
		{
			case false:
			{
				if (resObj is string str and "ok")
				{
					resStr = str;
					return true;
				}
				else
				{
					return false;
				}
			}

			case true:
			{
				if (
					resObj is string str &&
					str != "{}" &&
					JSRuntimeError.TryGet(resObj) == null &&
					resObj is string
				)
				{
					resStr = str;
					return true;
				}
				else
				{
					return false;
				}
			}
		}
	}

	public static JSErrorNfo GetError(CSErrorCtx ctx, object? resObj)
	{
		var runtimeErr = JSRuntimeError.TryGet(resObj);
		if (runtimeErr != null) return new JSErrorNfo(ctx, runtimeErr);

		var compileErr = JSCompilationError.TryGet(ctx.Code);
		if (compileErr != null) return new JSErrorNfo(ctx, compileErr);

		var returnError = JSWrongReturnType.TryGet(ctx.IsReturn, resObj);
		if (returnError != null) return new JSErrorNfo(ctx, returnError);

		return new JSErrorNfo(ctx, new JSUnknownError());
	}
}