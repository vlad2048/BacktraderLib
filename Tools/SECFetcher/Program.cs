using Feed.SEC;

namespace SECFetcher;

static class Program
{
	static void Main()
	{
		APIDev.LogRootFolder();
		APIDev.Run(Step.Clean | Step.Group);
	}
}