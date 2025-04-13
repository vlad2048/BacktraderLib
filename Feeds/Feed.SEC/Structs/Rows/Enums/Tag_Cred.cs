namespace Feed.SEC;

public enum Cred
{
	Credit,
	Debit,
}

static class CredUtils
{
	public static bool Is_Cred(this string s) => s is "" or "C" or "D";
	public static Cred? As_Cred(this string s) => s switch
	{
		"" => null,
		"C" => Cred.Credit,
		"D" => Cred.Debit,
		_ => throw new ArgumentException($"Unknown Cred: '{s}'"),
	};
	public static string Fmt_Cred(this Cred? s) => s switch
	{
		null => "",
		Cred.Credit => "C",
		Cred.Debit => "D",
		_ => throw new ArgumentException($"Unknown Cred: '{s}'"),
	};
}