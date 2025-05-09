using JetBrains.Annotations;

namespace LINQPadPlus;

public static partial class JS
{
	public static string Fmt(
		[LanguageInjection(InjectedLanguage.JAVASCRIPT)]
		string code,
		Func<string, string>? replFun = null
	)
		=> replFun switch
		{
			not null => replFun(code),
			null => code,
		};
}