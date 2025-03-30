namespace Feed.SEC;

sealed class StmtFieldAttribute(string displayName, bool bold, int level) : Attribute
{
	public string DisplayName => displayName;
	public bool Bold => bold;
	public int Level => level;
}