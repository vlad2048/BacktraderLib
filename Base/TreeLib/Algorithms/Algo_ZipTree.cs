public static class Algo_ZipTree
{
	public static TNod<(T, U)> ZipTree<T, U>(this TNod<T> rootA, TNod<U> rootB)
	{
		TNod<(T, U)> Recurse(TNod<T> nodeA, TNod<U> nodeB)
		{
			if (nodeA.Kids.Count != nodeB.Kids.Count)
				throw new ArgumentException("Cannot Zip trees with mismatched nodes");
			return Nod.Make(
				(nodeA.V, nodeB.V),
				nodeA.Kids.Zip(nodeB.Kids)
					.Select(t => Recurse(t.First, t.Second))
			);
		}

		return Recurse(rootA, rootB);
	}


	public static TNod<(TNod<T>, TNod<U>)> ZipTreeN<T, U>(this TNod<T> rootA, TNod<U> rootB)
	{
		TNod<(TNod<T>, TNod<U>)> Recurse(TNod<T> nodeA, TNod<U> nodeB)
		{
			if (nodeA.Kids.Count != nodeB.Kids.Count)
				throw new ArgumentException("Cannot Zip trees with mismatched nodes");
			return Nod.Make(
				(nodeA, nodeB),
				nodeA.Kids.Zip(nodeB.Kids)
					.Select(t => Recurse(t.First, t.Second))
			);
		}

		return Recurse(rootA, rootB);
	}
}