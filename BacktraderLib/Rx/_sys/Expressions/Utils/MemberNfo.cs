using System.Reflection;

namespace BacktraderLib._sys.Expressions.Utils;

sealed class MemberNfo(MemberInfo nfo)
{
	public override string ToString() => nfo switch
	{
		PropertyInfo propInfo => $"[{Type.Name}] {propInfo.Name} (p)",
		FieldInfo fieldInfo => $"[{Type.Name}] {fieldInfo.Name} (f)",
		_ => throw new ArgumentException()
	};

	public Type Type => nfo switch
	{
		PropertyInfo propInfo => propInfo.PropertyType,
		FieldInfo fieldInfo => fieldInfo.FieldType,
		_ => throw new ArgumentException()
	};

	public static MemberNfo[] GetNfosInType(Type type) => type
		.FindMembers(MemberTypes.Field | MemberTypes.Property, BindingFlags.Instance | BindingFlags.Public, null, null)
		.Select(nfo => new MemberNfo(nfo))
		.ToArray();
	
	public object GetValue(object parentObj) => nfo switch
	{
		PropertyInfo propInfo => propInfo.GetValue(parentObj)!,
		FieldInfo fieldInfo => fieldInfo.GetValue(parentObj)!,
		_ => throw new ArgumentException()
	};

	public void SetValue(object parentObj, object memberObj)
	{
		switch (nfo)
		{
			case PropertyInfo propInfo:
				propInfo.SetValue(parentObj, memberObj);
				break;
			case FieldInfo fieldInfo:
				fieldInfo.SetValue(parentObj, memberObj);
				break;
			default:
				throw new ArgumentException();
		}
	}
}
