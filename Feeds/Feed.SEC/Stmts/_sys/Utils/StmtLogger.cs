using System.Drawing;
using System.Reflection;
using BaseUtils;
using PrettyPrinting;

namespace Feed.SEC._sys.Utils;

static class StmtLogger
{
	static Color color(int v) => Color.FromArgb(0xff, Color.FromArgb(v));
	static readonly Color ColorBold = color(0xffffff);
	static readonly Color ColorNormal = color(0xaaaaaa);
	const int LevelIndent = 4;

	public static TxtArray Log<S>(this S stmt) where S : IStmt =>
		Txt.Build(t =>
		{

			foreach (var prop in stmt.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(e => e.PropertyType == typeof(decimal)))
			{
				var (displayName, bold, level) = prop.ReadAttribute<StmtFieldAttribute, (string, bool, int)>(e => (e.DisplayName, e.Bold, e.Level));
				var pad = new string(' ', level * LevelIndent);
				var val = (decimal)prop.GetValue(stmt)!;
				var left = $"{pad}{displayName}".PadRight(60);
				t.WriteLine($"{left}{val.Fmt()}", null, bold ? ColorBold : ColorNormal);
			}
		});



	static string Fmt(this decimal v)
	{
		if (v == 0)
			return "_".PadLeft(10);
		var str = Math.Abs(v) switch
		{
			>= 1_000_000_000 => $"${v / 1_000_000_000:F2}B",
			>= 1_000_000 => $"${v / 1_000_000:F2}M",
			>= 1_000 => $"${v / 1_000:F2}K",
			_ => $"${v:F2} ",
		};
		return str.PadLeft(10);
	}
}