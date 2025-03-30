public static class Algo_Map
{
	public static TNod<U> Map<T, U>(this TNod<T> root, Func<T, U> mapFun) => root.MapN((nod, _) => mapFun(nod.V));
	public static TNod<U> Map<T, U>(this TNod<T> root, Func<T, int, U> mapFun) => root.MapN((nod, lvl) => mapFun(nod.V, lvl));
	public static TNod<U> MapN<T, U>(this TNod<T> root, Func<TNod<T>, U> mapFun) => root.MapN((nod, _) => mapFun(nod));

	public static TNod<U> MapN<T, U>(this TNod<T> root, Func<TNod<T>, int, U> mapFun)
	{
		TNod<U> Recurse(TNod<T> node, int lvl) => Nod.Make(mapFun(node, lvl), node.Kids.Select(e => Recurse(e, lvl + 1)));
		return Recurse(root, 0);
	}


	/// <summary>
	/// Map a tree <br/>
	/// Also gives the selector function access to the absolute index of the node in the tree
	/// </summary>
	public static TNod<U> MapNIdx<T, U>(this TNod<T> root, Func<TNod<T>, int, U> mapFun)
	{
		var idx = 0;
		TNod<U> Recurse(TNod<T> node) => Nod.Make(mapFun(node, idx++), node.Kids.Select(Recurse));
		return Recurse(root);
	}
}