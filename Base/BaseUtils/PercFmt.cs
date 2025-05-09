namespace BaseUtils;

public static class PercFmt
{
	public static string perc(this int idx, int cnt) => $"{idx}/{cnt} " + cnt switch
	{
		0 => "(_)",
		_ => $"({idx * 100.0 / cnt:F2}%)",
	};
}