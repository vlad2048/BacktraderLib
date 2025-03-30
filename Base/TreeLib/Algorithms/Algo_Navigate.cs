public static class Algo_Navigate
{
	public static TNod<T> GoUpToRoot<T>(this TNod<T> nod)
		=>
			nod.GoUpToRootOrUntilN(_ => false);

	public static TNod<T> GoUpToRootOrUntil<T>(this TNod<T> nod, Func<T, bool> predicate)
		=>
			nod.GoUpToRootOrUntilN(n => predicate(n.V));

	public static TNod<T> GoUpToRootOrUntilN<T>(this TNod<T> nod, Func<TNod<T>, bool> predicate)
	{
		var curNod = nod;
		while (curNod.Dad != null && !predicate(curNod))
			curNod = curNod.Dad;
		return curNod;
	}
}