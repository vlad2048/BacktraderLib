using System.Drawing;

namespace PrettyPrinting._sys;

static class TxtBuilderLogic
{
	public static TxtArray Build(Action<ITxtBuilder> action)
	{
		var printer = new TxtBuilder();
		action(printer);
		return printer.Result;
	}

	sealed class TxtBuilder : ITxtBuilder
	{
		readonly List<Txt> arr = [];
		public TxtArray Result => new([.. arr]);
		public void Write(string text, Color? back, Color? fore) => arr.Add(new Txt(text, back, fore));
		public void WriteLine() => arr.Add(Txt.Newline);
	}
}