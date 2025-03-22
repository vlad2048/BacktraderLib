namespace BacktraderLib._sys;

static class FileUtils
{
	public static string[] GetGlobFiles(string glob) =>
		glob.Contains('*') switch
		{
			false => [glob.EnsureFileExists()],
			true => Directory.GetFiles(Path.GetDirectoryName(glob)!, Path.GetFileName(glob)),
		};

	static string EnsureFileExists(this string file)
	{
		if (!File.Exists(file)) throw new ArgumentException($"Cannot find file: '{file}'");
		return file;
	}
}