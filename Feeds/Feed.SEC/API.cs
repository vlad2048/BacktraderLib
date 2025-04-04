using BaseUtils;
using Feed.SEC._sys.Logic;

namespace Feed.SEC;

public static class API
{
	public static void DownloadAndProcessSECData() => APIDev.Run();

	public static IReadOnlyDictionary<Quarter, DateOnly> MapQuartersToFiledDates(string company, Quarter[] quarters) => FiledDateMapper.Map(company, quarters);
}