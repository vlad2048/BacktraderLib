namespace BaseUtils;

public static class DecimalFmtHumanUtils
{
	public static string FmtHuman(this decimal e) =>
		e switch
		{
			>= 1_000_000_000 => $"{e / 1_000_000_000:n2}B",
			>= 1_000_000 => $"{e / 1_000_000:n2}M",
			>= 1_000 => $"{e / 1_000:n2}K",
			_ => $"{e:n2}",
		};
}