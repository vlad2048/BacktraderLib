using BacktraderLib._sys.JsonConverters;

namespace BacktraderLib;


public enum SearchType
{
	TextBox,
	DropDown,
}


[PlotlyEnum(EnumStyle.CamelCase)]
public enum TableLayout
{
	FitData,
	FitColumns,
	FitDataFill,
	FitDataStretch,
	FitDataTable,
}


[PlotlyEnum(EnumStyle.CamelCase)]
public enum ColumnAlign
{
	Left,
	Center,
	Right,
}
