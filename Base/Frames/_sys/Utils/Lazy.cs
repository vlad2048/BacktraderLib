namespace Frames._sys.Utils;

static class Lazy
{
	public static Lazy<T> Make<T>(Func<T> fun) => new(fun);
}