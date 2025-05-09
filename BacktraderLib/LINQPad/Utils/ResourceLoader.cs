namespace BacktraderLib.Utils;

public static class ResourceLoader
{
	public static string Load(string resourceName)
	{
		var ass = typeof(ResourceLoader).Assembly;
		using var stream = ass.GetManifestResourceStream(resourceName) ?? throw new ArgumentException($"Cannot find resource: '{resourceName}'");
		using var reader = new StreamReader(stream);
		return reader.ReadToEnd();
	}
}