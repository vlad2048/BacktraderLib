using System.Collections;
using _sys.Logging;
using PrettyPrinting;

public static class Nod
{
	public static TNod<T> Make<T>(T v, IEnumerable<TNod<T>>? kids = null) => new(v, kids);
	public static TNod<T> Make<T>(T v, params TNod<T>[] kids) => new(v, kids);
}


public sealed class TNod<T> : IEnumerable<TNod<T>>, ITxt
{
	readonly List<TNod<T>> kids = [];

	public T V { get; }
	public TNod<T>? Dad { get; private set; }
	public IReadOnlyList<TNod<T>> Kids => kids;

	internal TNod(T v, IEnumerable<TNod<T>>? kids)
	{
		V = v;
		if (kids != null)
			foreach (var kid in kids)
			{
				this.kids.Add(kid);
				kid.Dad = this;
			}
	}

	public override string ToString()
	{
		try
		{
			return $"{V}";
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
	}

	public void AddKid(TNod<T> kid)
	{
		kids.Add(kid);
		kid.Dad = this;
	}

	public void InsertKid(TNod<T> kid, int index)
	{
		kids.Insert(index, kid);
		kid.Dad = this;
	}

	public void RemoveKid(TNod<T> kid)
	{
		kid.Dad = null;
		kids.Remove(kid);
	}

	public void ReplaceKid(TNod<T> kidPrev, TNod<T> kidNext)
	{
		var index = kids.IndexOf(kidPrev);
		if (index == -1) throw new ArgumentException();
		kidPrev.Dad = null;
		kids[index] = kidNext;
		kidNext.Dad = this;
	}

	public void ClearKids()
	{
		foreach (var kid in Kids)
			kid.Dad = null;
		kids.Clear();
	}

	public void AddKids(IEnumerable<TNod<T>> kids_)
	{
		foreach (var kid in kids_)
			AddKid(kid);
	}

	public IEnumerator<TNod<T>> GetEnumerator() => Enumerate();
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	IEnumerator<TNod<T>> Enumerate()
	{
		IEnumerable<TNod<T>> Recurse(TNod<T> node)
		{
			yield return node;
			foreach (var kid in node.Kids)
			foreach (var kidRes in Recurse(kid))
				yield return kidRes;
		}
		foreach (var res in Recurse(this))
			yield return res;
	}


	// Printing
	// --------
	public object ToDump() => Txt;
	public TxtArray Txt => TreeLogger.Log(this);
}