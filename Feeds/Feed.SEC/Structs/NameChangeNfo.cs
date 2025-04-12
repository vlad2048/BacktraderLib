namespace Feed.SEC;

public sealed record NameChangeInfos(
	Dictionary<string, DateOnly> LastFiledDates,
	NameChangeNfo[] Changes
);

public sealed record NameChangeNfo(
	string Name,
	string Former,
	DateOnly Changed
)
{
	public object ToDump() => new
	{
		Name,
		Former,
		Changed,
	};
}