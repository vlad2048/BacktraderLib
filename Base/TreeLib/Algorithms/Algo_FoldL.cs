public static class Algo_FoldL
{
	/// <summary>
	/// Map a tree recursively.
	/// For each node, we use the node and the mapped dad as input
	/// </summary>
	public static TNod<U> FoldL<T, U>(
		this TNod<T> root,
		Func<TNod<T>, U, U> fun,
		U seed
	)
	{
		TNod<U> Recurse(TNod<T> node, U mayMappedDadVal)
		{
			var mappedNodeVal = fun(node, mayMappedDadVal);
			var mappedKids = node.Kids.Select(kid => Recurse(kid, mappedNodeVal));
			var mappedNode = Nod.Make(mappedNodeVal, mappedKids);
			return mappedNode;
		}

		return Recurse(root, seed);
	}
}