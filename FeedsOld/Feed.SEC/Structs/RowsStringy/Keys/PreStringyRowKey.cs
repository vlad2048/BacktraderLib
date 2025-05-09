namespace Feed.SEC;

public sealed record PreStringyRowKey(
	SubStringyRowKey SubKey,
	string Report,
	string Line
);