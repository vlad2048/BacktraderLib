namespace Feed.TwelveData._sys.Structs;

enum Plan
{
	None,
	Basic,
	Grow,
	Pro,
	Enterprise,
}

static class PlanUtils
{
	public static Access ToAccess(Plan e) => throw new NotImplementedException("Dummy");
	public static Plan ToPlan(Access? e) => e switch
	{
		null => Plan.None,
		not null => e.Plan switch
		{
			"Basic" => Plan.Basic,
			"Grow" => Plan.Grow,
			"Pro" => Plan.Pro,
			"Enterprise" => Plan.Enterprise,
			_ => throw new ArgumentException($"Unknown plan: {e.Plan}"),
		}
	};
}
