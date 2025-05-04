using System.Reflection;
using BaseUtils;

namespace BacktraderLib._sys.Utils;

static class ColumnGuesser
{
	public static ColumnOptions<T>[] Guess<T>() =>
		typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
			.SelectA(prop =>
				new ColumnOptions<T>(
					GuessFun<T>(prop),
					GuessName<T>(prop),
					prop.PropertyType
				)
				.GuessAlign(prop)
			);


	static Func<T, object> GuessFun<T>(PropertyInfo prop)
	{
		if (prop.PropertyType == typeof(decimal))
			return item => ((decimal)prop.GetValue(item)!).FmtHuman();
		return item => prop.GetValue(item) ?? throw new ArgumentException("GetValue returned null");
	}

	static string GuessName<T>(PropertyInfo prop) => prop.Name;

	static ColumnOptions<T> GuessAlign<T>(this ColumnOptions<T> opt, PropertyInfo prop)
	{
		if (prop.PropertyType == typeof(decimal))
			opt.Align(ColumnAlign.Right);
		return opt;
	}
}