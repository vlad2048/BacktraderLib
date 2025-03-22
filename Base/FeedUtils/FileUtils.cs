namespace FeedUtils;

public static class FileUtils
{
	public static string GetProjectRootFolder(string projectName) => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), projectName).CreateFolderIFN();

	public static string CreateFolderIFN(this string e)
	{
		if (!Directory.Exists(e))
			Directory.CreateDirectory(e);
		return e;
	}
}