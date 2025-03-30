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

	public static void EmptyFolder(string folder)
	{
		foreach (var file in Directory.GetFiles(folder)) File.Delete(file);
		foreach (var subFolder in Directory.GetDirectories(folder)) Directory.Delete(subFolder, true);
	}
}