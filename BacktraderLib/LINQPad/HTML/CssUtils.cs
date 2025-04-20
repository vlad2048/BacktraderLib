using JetBrains.Annotations;
using LINQPad;

namespace BacktraderLib;

static class CssUtils
{
	public static void AddStyles([LanguageInjection(InjectedLanguage.CSS)] string css) => Util.HtmlHead.AddStyles(css);
}