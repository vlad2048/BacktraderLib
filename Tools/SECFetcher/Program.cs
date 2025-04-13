using Feed.SEC;

namespace SECFetcher;

static class Program
{
	static void Main()
	{
		API.Fetcher.Run(Step.Clean | Step.Group);
	}
}