namespace LINQPadPlus._sys;

static class IdGen
{
	/// <summary>
	/// Generate a new unique html element id.
	/// In particular it can be used with document.querySelector as it starts with a letter.
	/// </summary>
	public static string Make() => $"id_{Guid.NewGuid()}".Replace("-", "");
}