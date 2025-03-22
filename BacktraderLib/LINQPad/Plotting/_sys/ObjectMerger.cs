using System.Reflection;

namespace BacktraderLib._sys;

static class ObjectMerger
{
	public static T Merge<T>(T objA, T objB)
	{
		if (objA == null) throw new ArgumentNullException(nameof(objA));
		if (objB == null) throw new ArgumentNullException(nameof(objB));

		var result = Activator.CreateInstance<T>();

		foreach (var property in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
		{
			if (property.GetSetMethod() != null)
			{
				var valueA = property.GetValue(objA);
				var valueB = property.GetValue(objB);
				property.SetValue(result, valueB ?? valueA);
			}
		}

		return result;
	}


	public static T MergeOpt<T>(T objA, T? objB) where T : class
	{
		if (objA == null) throw new ArgumentNullException(nameof(objA));
		if (objB == null) return objA;
		return Merge(objA, objB);
	}
}