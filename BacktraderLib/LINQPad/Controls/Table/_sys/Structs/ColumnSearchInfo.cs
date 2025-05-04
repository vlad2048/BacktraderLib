namespace BacktraderLib._sys.Structs;

sealed record ColumnSearchInfo<T>(
	Func<T, object>? FunOverride
);