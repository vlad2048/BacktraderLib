using RxLib;

namespace BacktraderLib;

public static class CtrlsExt
{
	public static Tag WithLabel(this Tag tag, string label) =>
		new("label", null, label)
		{
			Kids =
			[
				tag,
			],
		};

	public static Tag WithHeader(this Tag tag, string label) =>
		new("div")
		{
			Class = "heaadingpresenter",
			Kids =
			[
				new Tag("h1", null, label)
				{
					Class = "headingpresenter",
				},
				tag,
			],
		};

	public static (IRoVar<T>, Tag) WithLabel<T>(this (IRoVar<T>, Tag) t, string label) =>
	(
		t.Item1,
		t.Item2.WithLabel(label)
	);
}