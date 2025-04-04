using HtmlAgilityPack;
using Microsoft.Playwright;

namespace WebScript._sys.Utils;

static class DomTreeUtils
{
	public static async Task<TNod<string>> GetDataTestIdTree(this IPage page)
	{
		var html = await page.ContentAsync();
		var doc = new HtmlDocument();
		doc.LoadHtml(html);
		return doc.DocumentNode.ToTree();
	}

	static TNod<string> ToTree(this HtmlNode root)
	{
		IEnumerable<TNod<string>> Rec(HtmlNode node)
		{
			var attr = node.GetDataAttribute("testid");
			if (attr != null)
			{
				return [Nod.Make(attr.Value, node.ChildNodes.SelectMany(Rec))];
			}
			else
			{
				return node.ChildNodes.SelectMany(Rec);
			}
		}
		return Nod.Make("(root)", root.ChildNodes.SelectMany(Rec));
	}
}