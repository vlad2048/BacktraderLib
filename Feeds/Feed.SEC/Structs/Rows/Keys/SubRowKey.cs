namespace Feed.SEC;

public sealed record SubRowKey(string Adsh)
{
	public override string ToString() => Adsh;
	public object ToDump() => $"{this}";
}