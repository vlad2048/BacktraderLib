using LINQPad.Controls;

namespace BacktraderLib._sys;

static class CssExt
{
	public static C With<C>(this C ctrl, params string[] kvs) where C : Control
	{
		foreach (var kv in kvs)
		{
			var (key, val) = Chop(kv, ':');
			ctrl.Styles[key] = val;
		}
		return ctrl;
	}



	static (string, string) Chop(this string s, char ch)
	{
		var parts = s.Split(new[] { ch }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
		if (parts.Length != 2) throw new ArgumentException();
		return (parts[0], parts[1]);
	}
}