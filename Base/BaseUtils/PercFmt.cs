namespace BaseUtils;

public static class PercFmt
{
	public static string perc(this int idx, int cnt) => $"{idx + 1}/{cnt} " + cnt switch
	{
		0 => "(_)",
		_ => $"({(idx + 1) * 100.0 / cnt:F2}%)",
	};
}