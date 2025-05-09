namespace Feed.Trading212;

public sealed record Template(Dictionary<ReportType, FieldDef[]> Reports);