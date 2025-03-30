public static class Algo_LimitDepth
{
	public static TNod<T> LimitDepth<T>(this TNod<T> root, int maxDepth)
	{
		if (maxDepth < 0) throw new ArgumentException();
		return root.Filter((_, lvl) => lvl <= maxDepth).Single();
	}



	static TNod<T>[] Filter<T>(this TNod<T> root, Func<T, int, bool> predicate) => root.FilterN((n, lvl) => predicate(n.V, lvl));

	static TNod<T>[] FilterN<T>(this TNod<T> root, Func<TNod<T>, int, bool> predicate)
	{
		bool Predicate(TNod<T> node, int lvl) => predicate(node, lvl) || node == root;
		return root.Filter_KeepIfMatchingOnly(Predicate);
	}

	static TNod<T>[] Filter_KeepIfMatchingOnly<T>(this TNod<T> root, Func<TNod<T>, int, bool> predicate)
	{
		IEnumerable<TNod<T>> FindMatchingKids(TNod<T> topNode, bool includeTopNodeIfMatch, int lvl)
		{
			if (includeTopNodeIfMatch && predicate(topNode, lvl))
				return [topNode];

			var filteredKids = new List<TNod<T>>();

			void Recurse(TNod<T> _node, int _lvl)
			{
				foreach (var kid in _node.Kids)
				{
					if (predicate(kid, _lvl))
					{
						var filteredKid = BuildRecurse(kid, _lvl);
						filteredKids.Add(filteredKid);
					}
					else
					{
						Recurse(kid, _lvl + 1);
					}
				}
			}

			Recurse(topNode, lvl + 1);
			return filteredKids;
		}

		TNod<T> BuildRecurse(TNod<T> node, int lvl) => Nod.Make(node.V, FindMatchingKids(node, false, lvl));

		var outputNodes = FindMatchingKids(root, true, 0).Select(BuildRecurse).ToArray();

		return outputNodes;
	}
}