namespace BacktraderLib._sys;

public static class Lazy
{
	public static Lazy<T> Make<T>(Func<T> fun) => new(fun);
}