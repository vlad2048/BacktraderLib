using BaseUtils;

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

	// While the '.' is valid in filenames and folder names,
	// it is NOT possible to use it at the end of filenames and folder names
	public static string ToFileSafe(this string e) => e
		.Replace('/', '+')
		.Replace('\\', '=')
		.Replace(':', '@')
		.Replace('.', '~');
	public static string FromFileSafe(this string e) => e
		.Replace('+', '/')
		.Replace('=', '\\')
		.Replace('@', ':')
		.Replace('~', '.');

	public static string[] FromAllFilesSafe(this string[] files) => files.SelectA(e => (Path.GetFileNameWithoutExtension(e) ?? throw new ArgumentException("Oh no")).FromFileSafe());


	public static bool Is_Dst_MissingOrNotAsRecentAs_Src(string fileSrc, string fileDst)
	{
		if (!File.Exists(fileSrc)) throw new IOException($"fileSrc is missing: '{fileSrc}'");
		if (!File.Exists(fileDst)) return true;
		var timeSrc = new FileInfo(fileSrc).LastWriteTime;
		var timeDst = new FileInfo(fileDst).LastWriteTime;
		return timeDst < timeSrc;
	}

	public static bool DoesFileExistAndIsRecentEnough(string file, TimeSpan maxAge) =>
			File.Exists(file) &&
			DateTime.Now - new FileInfo(file).LastWriteTime <= maxAge;
}