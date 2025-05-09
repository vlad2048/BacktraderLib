namespace LINQPadPlus;

public sealed record CSharpSourceLocation(
	string? Member,
	string? File,
	int Line
);