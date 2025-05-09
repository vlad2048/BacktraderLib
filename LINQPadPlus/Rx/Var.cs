namespace LINQPadPlus.Rx;

public static class Var
{
	public static RwVar<T> Make<T>(T init) => new(init, false);
}