public static class Algo_FoldR
{
	/// <summary>
	/// Map a tree recursively.
	/// For each node, we use the node and the mapped kids as input
	/// </summary>
	public static TNod<U> FoldR<T, U>(
		this TNod<T> root,
		Func<T, IReadOnlyList<U>, U> fun
	)
	{
		TNod<U> Recurse(TNod<T> node)
		{
			var foldedKids = node.Kids
				.Select(Recurse).ToArray();
			var foldedNode = Nod.Make(
				fun(node.V, foldedKids.Select(e => e.V).ToArray()),
				foldedKids
			);
			return foldedNode;
		}

		return Recurse(root);
	}
}