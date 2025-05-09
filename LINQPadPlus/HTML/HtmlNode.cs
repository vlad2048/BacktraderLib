using LINQPad;
using LINQPad.Controls;
using LINQPadPlus._sys.TagConverters;

namespace LINQPadPlus;

public enum HtmlNodeType
{
	Tag,
	Text,
	Control,
	DumpContainer,
	Empty,
	// ------
	Group,
}

public sealed record EmptyHtmlNode
{
	internal static readonly EmptyHtmlNode Instance = new();
}

public sealed class HtmlNode
{
	public HtmlNodeType Type { get; }
	public Tag? Tag { get; }
	public string? Text { get; }
	public HtmlNode[]? Group { get; }
	public Control? Control { get; }
	public DumpContainer? DumpContainer { get; }
	
	HtmlNode(Tag tag) => (Type, Tag) = (HtmlNodeType.Tag, tag);
	HtmlNode(string text) => (Type, Text) = (HtmlNodeType.Text, text);
	HtmlNode(Control control) => (Type, Control) = (HtmlNodeType.Control, control);
	HtmlNode(DumpContainer dumpContainer) => (Type, DumpContainer) = (HtmlNodeType.DumpContainer, dumpContainer);
	HtmlNode() => Type = HtmlNodeType.Empty;
	// ------
	HtmlNode(HtmlNode[] group) => (Type, Group) = (HtmlNodeType.Group, group);
	
	public static implicit operator HtmlNode(Tag tag) => new(tag);
	public static implicit operator HtmlNode(string text) => new(text);
	public static implicit operator HtmlNode(Control control) => new(control);
	public static implicit operator HtmlNode(DumpContainer dumpContainer) => new(dumpContainer);
	public static implicit operator HtmlNode(EmptyHtmlNode _) => new();
	// ------
	public static implicit operator HtmlNode(HtmlNode[] group) => new(group.Select(e => e).ToArray());
	public static implicit operator HtmlNode(Tag[] group) => new(group.Select(e => (HtmlNode)e).ToArray());
	public static implicit operator HtmlNode(string[] group) => new(group.Select(e => (HtmlNode)e).ToArray());
	public static implicit operator HtmlNode(Control[] group) => new(group.Select(e => (HtmlNode)e).ToArray());
	public static implicit operator HtmlNode(DumpContainer[] group) => new(group.Select(e => (HtmlNode)e).ToArray());
	
	public override string ToString() => RenderString(false);
	
	internal string RenderString(bool runJs) => Type switch
	{
		HtmlNodeType.Tag => Tag!.RenderString(runJs),
		HtmlNodeType.Text => $"{Text}",
		HtmlNodeType.Empty => string.Empty,
		HtmlNodeType.Control => Control!.ToTag().RenderString(runJs),
		HtmlNodeType.DumpContainer => DumpContainer!.ToTag().RenderString(runJs),
		// ------
		HtmlNodeType.Group => string.Join("", Group!.Select(e => e.RenderString(runJs))),
		_ => throw new ArgumentException("Impossible"),
	};
}


public static class HtmlNodeUtils
{
	public static HtmlNode If(this Tag tag, bool condition) => condition ? tag : EmptyHtmlNode.Instance;
	public static HtmlNode If(this string text, bool condition) => condition ? text : EmptyHtmlNode.Instance;
	public static HtmlNode If(this Control control, bool condition) => condition ? control : EmptyHtmlNode.Instance;
	public static HtmlNode If(this DumpContainer dumpContainer, bool condition) => condition ? dumpContainer : EmptyHtmlNode.Instance;
	// ------
	public static HtmlNode If(this HtmlNode[] group, bool condition) => condition ? group : EmptyHtmlNode.Instance;
	public static HtmlNode If(this Tag[] group, bool condition) => condition ? group : EmptyHtmlNode.Instance;
	public static HtmlNode If(this string[] group, bool condition) => condition ? group : EmptyHtmlNode.Instance;
	public static HtmlNode If(this Control[] group, bool condition) => condition ? group : EmptyHtmlNode.Instance;
	public static HtmlNode If(this DumpContainer[] group, bool condition) => condition ? group : EmptyHtmlNode.Instance;
}