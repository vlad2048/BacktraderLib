public static class Algo_IsEqual
{
	public static bool IsEqual<T>(this TNod<T> nodeA, TNod<T> nodeB)
	{
		if (!Equals(nodeA.V, nodeB.V) || nodeA.Kids.Count != nodeB.Kids.Count)
			return false;
		return nodeA.Kids.Zip(nodeB.Kids)
			.All(t => IsEqual(t.First, t.Second));
	}
}