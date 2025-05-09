namespace Feed.SEC;

public sealed record PreRowKey(
	SubRowKey SubKey,
	int Report,
	int Line
);