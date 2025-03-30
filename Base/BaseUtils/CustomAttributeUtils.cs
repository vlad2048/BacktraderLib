using System.Reflection;

namespace BaseUtils;

public static class CustomAttributeUtils
{
	public static T ReadAttribute<A, T>(this PropertyInfo prop, Func<A, T> valFun) where A : Attribute =>
		prop.GetCustomAttribute(typeof(A)) switch
		{
			A attr => valFun(attr),
			_ => throw new ArgumentException($"{typeof(A).Name} not found on {prop.DeclaringType?.Name}.{prop.Name}"),
		};
}