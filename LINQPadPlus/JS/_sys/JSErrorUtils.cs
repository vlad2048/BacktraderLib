using System.Diagnostics.CodeAnalysis;
using LINQPadPlus._sys.Structs;
using LINQPadPlus._sys.Utils;

namespace LINQPadPlus._sys;

static class JSErrorUtils
{
	public static bool TryGetReturnString(object? resObj, CSErrorCtx ctx, [NotNullWhen(true)] out string? resStr)
	{
		resStr = null;
		switch (ctx.IsReturn)
		{
			case false:
			{
				if (resObj is JSRunIdentifiers.ValidRunReturnValueIdentifier)
				{
					resStr = JSRunIdentifiers.ValidRunReturnValueIdentifier;
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
					JSErrorFinder.CheckRuntimeError(resObj) == null &&
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
}