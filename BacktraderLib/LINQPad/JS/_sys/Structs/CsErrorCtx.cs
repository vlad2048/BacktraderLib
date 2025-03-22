namespace BacktraderLib._sys.Structs;

sealed record CSErrorCtx(
	bool IsReturn,
	string Code,
	string CodeFull,
	string? SrcMember,
	string? SrcFile,
	int SrcLine
);
