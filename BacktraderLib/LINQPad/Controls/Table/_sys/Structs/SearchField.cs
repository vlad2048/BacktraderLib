namespace BacktraderLib._sys.Structs;

sealed record SearchField<T>(
	Func<T, object> Fun,
	string Name
);