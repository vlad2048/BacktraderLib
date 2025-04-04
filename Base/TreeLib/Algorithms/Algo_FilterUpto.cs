public static class Algo_FilterUpto
{
	/*
	
	FilterUpto(e => e == "X")
	=========================

	         ┌─ X ─ n                ┌─ X
	   ┌─ n ─┤                 ┌─ n ─┘
	r ─┤     └─ n      ==>  r ─┘
	   └─ n

	*/

	public static TNod<T> FilterUpto<T>(this TNod<T> root, Func<T, bool> predicate)
	{
		var set = root.Where(e => predicate(e.V)).ToHashSet();
		foreach (var nod in set.ToArray())
		{
			var cur = nod.Dad;
			while (cur != null)
			{
				set.Add(cur);
				cur = cur.Dad;
			}
		}

		TNod<T> Rec(TNod<T> nod) => Nod.Make(nod.V, nod.Kids.Where(set.Contains).Select(Rec));

		return Rec(root);
	}
}