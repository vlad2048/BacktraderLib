using BacktraderLib._sys.JsonConverters;

namespace BacktraderLib;

[PlotlyEnum(EnumStyle.CamelCase)]
public enum TableLayout
{
	FitData,
	FitColumns,
	FitDataFill,
	FitDataStretch,
	FitDataTable,
}