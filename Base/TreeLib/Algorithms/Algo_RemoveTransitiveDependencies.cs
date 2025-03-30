public static class Algo_RemoveTransitiveDependencies
{
	public static TNod<T> RemoveTransitiveDependencies<T>(this TNod<T> root, IEqualityComparer<T>? cmpFun = null)
	{
		var cmp = cmpFun ?? EqualityComparer<T>.Default;
		var descendentMap = root.ToDictionary(e => e, e => GetAllDescendents(e, cmp));
		bool IsInDescendents(IEnumerable<TNod<T>> dads, TNod<T> kid) => dads.Any(dad => descendentMap[dad].Contains(kid.V));

		TNod<T> Rec(TNod<T> n)
		{
			var listTodo = new Queue<TNod<T>>(n.Kids);
			var listDone = new List<TNod<T>>();
			var dads = n.GetDads(cmp);

			while (listTodo.TryDequeue(out var kid))
			{
				var isSomewhereElse =
					dads.Contains(kid.V) ||
					IsInDescendents(listTodo, kid) ||
					IsInDescendents(listDone, kid);
				if (!isSomewhereElse)
					listDone.Add(kid);
			}

			return Nod.Make(n.V, listDone.Select(Rec));
		}

		return Rec(root);
	}

	static HashSet<T> GetAllDescendents<T>(this TNod<T> root, IEqualityComparer<T> cmp) => new(root.Skip(1).Select(e => e.V), cmp);

	static HashSet<T> GetDads<T>(this TNod<T> root, IEqualityComparer<T> cmp)
	{
		var set = new HashSet<T>(cmp);
		var node = root.Dad;
		while (node != null)
		{
			set.Add(node.V);
			node = node.Dad;
		}
		return set;
	}
}