using BaseUtils;
using Feed.Trading212._sys._1_Scraping.Utils;
using Feed.Trading212._sys._2_Processing.Logic;
using FeedUtils;

namespace Feed.Trading212._sys._2_Processing;

static class Processor
{
	public static void Process()
	{
		var template = LoadTemplate();

		Consts.Scraping.GetAllCompanies()
			.ShowProgress()
			.Select(companyName => new
			{
				CompanyName = companyName,
				FileSrc = Consts.Scraping.CompanyJsonFile(companyName),
				FileDst = Consts.Processing.CompanyJsonFile(companyName),
			})
			.Where(t => API.Scraping.GetCompanyScrapeState(t.CompanyName).Status is CompanyScrapeStatus.Success)
			.Where(x => FileUtils.Is_Dst_MissingOrNotAsRecentAs_Src(x.FileSrc, x.FileDst))
			.ForEach(t =>
			{
				var dataSrc = API.Scraping.Load(t.CompanyName);
				var dataDst = ReportCompiler.Compile(dataSrc, template, t.CompanyName);
				JsonUtils.SaveOptimized(dataDst, t.FileDst);
			});
	}


	public static Template LoadTemplate() => JsonUtils.LoadOrLazy(
		Consts.Processing.TemplateFile,
		() => JsonUtils.Save(TemplateExtractor.Extract(), Consts.Processing.TemplateFile)
	);
}