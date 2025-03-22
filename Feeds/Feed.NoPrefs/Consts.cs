using Feed.NoPrefs._sys.DoltLogic.Structs;
using FeedUtils;

namespace Feed.NoPrefs;

static class Consts
{
	static readonly string RootFolder = FileUtils.GetProjectRootFolder("Feed.NoPrefs");
	static readonly string DBFolder = Path.Combine(RootFolder, "_dbs").CreateFolderIFN();

	internal static readonly string DataFolder = Path.Combine(RootFolder, "Data").CreateFolderIFN();


	public const string DoltExe = "dolt";
	public static readonly TimeSpan FetchDelay = TimeSpan.FromHours(24);
	public static string GetUserFolder(this DbNfo db) => Path.Combine(DBFolder, db.User).CreateFolderIFN();
	public static string GetFolder(this DbNfo db)
	{
		var userFolder = db.GetUserFolder();
		return Path.Combine(userFolder, db.Name);
	}
}