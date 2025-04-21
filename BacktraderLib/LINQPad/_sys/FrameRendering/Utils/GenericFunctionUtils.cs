using System.Reflection;

namespace BacktraderLib._sys.FrameRendering.Utils;

static class GenericFunctionUtils
{
	public static R Call<R>(MethodInfo method, object obj)
	{
		var metArgs = method.GetGenericArguments();
		var objArgs = obj.GetType().GenericTypeArguments;
		if (metArgs.Length != objArgs.Length) throw new ArgumentException("Incompatible method");
		var methodFinal = method.MakeGenericMethod(objArgs);
		var retObj = methodFinal.Invoke(null, [obj]);
		if (retObj is not R ret) throw new ArgumentException("Wrong return type");
		return ret;
	}
}