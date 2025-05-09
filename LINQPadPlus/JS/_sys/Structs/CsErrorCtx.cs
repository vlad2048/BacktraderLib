namespace LINQPadPlus._sys.Structs;

sealed record CSErrorCtx(
	bool IsReturn,
	string Code,
	string CodeFull,
	CSharpSourceLocation CSharpSourceLocation
);
