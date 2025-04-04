using System.Text.Json.Nodes;
using BaseUtils;

namespace WebScript.Structs;


public record SymbolReportData(SortedDictionary<Quarter, JsonArray> Quarters);

public record SymbolData(Dictionary<ReportType, SymbolReportData> Reports);