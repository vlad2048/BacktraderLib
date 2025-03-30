using BaseUtils;

public static class Algo_Filter
{
	public static TNod<T> Filter<T>(this TNod<T> root, Func<T, bool> predicate)
	{
		TNod<T>[] Rec(TNod<T> node) => (predicate(node.V) || node == root) switch
		{
			true => [Nod.Make(node.V, node.Kids.SelectMany(Rec))],
			false => node.Kids.SelectManyA(Rec),
		};

		return Rec(root).Single();
	}

	public static TNod<T> FilterN<T>(this TNod<T> root, Func<TNod<T>, bool> predicate)
	{
		TNod<T>[] Rec(TNod<T> node) => (predicate(node) || node == root) switch
		{
			true => [Nod.Make(node.V, node.Kids.SelectMany(Rec))],
			false => node.Kids.SelectManyA(Rec),
		};

		return Rec(root).Single();
	}
}
