namespace Feed.SEC;

[Flags]
public enum Step
{
	Download = 1,
	Clean = 2,
	Group = 4,
	Rename = 8,

	All = Download | Clean | Group | Rename,
}