using System.Reflection;

namespace BacktraderLib._sys;

static class ObjectMerger
{
	public static T Merge<T>(T objA, T objB)
	{
		if (objA == null) throw new ArgumentNullException(nameof(objA));
		if (objB == null) throw new ArgumentNullException(nameof(objB));
		if (objA.GetType() != objB.GetType()) throw new ArgumentException("Mismatched trace types");
		var type = objA.GetType();

		var result = Activator.CreateInstance(type);

		foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
		{
			if (property.GetSetMethod() != null)
			{
				var valueA = property.GetValue(objA);
				var valueB = property.GetValue(objB);
				property.SetValue(result, valueB ?? valueA);
			}
		}

		return (T)result!;
	}

	public static T MergeOpt<T>(T objA, T? objB) where T : class
	{
		if (objA == null) throw new ArgumentNullException(nameof(objA));
		if (objB == null) return objA;
		return Merge(objA, objB);
	}
}