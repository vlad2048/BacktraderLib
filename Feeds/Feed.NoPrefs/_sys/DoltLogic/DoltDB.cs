using BaseUtils;
using Feed.NoPrefs._sys.DoltLogic.Structs;
using Feed.NoPrefs._sys.Utils;
using MySqlConnector;

namespace Feed.NoPrefs._sys.DoltLogic;


// TODO public
public sealed class DoltDB(DbNfo nfo)
{
	// ***********
	// * Private *
	// ***********
	MySqlConnection? conn_;
	DateTime lastPullTime_;
	MySqlConnection Conn
	{
		get
		{
			InitIFN();
			return conn_ ?? throw new ArgumentException("Shouldn't be null here");
		}
		set => conn_ = value;
	}




	// **********
	// * Public *
	// **********
	public DateTime LastPullTime
	{
		get
		{
			InitIFN();
			return lastPullTime_;
		}
		private set => lastPullTime_ = value;
	}


	public T[] Read<T>(string sql) => Conn.Read<T>(sql);
	public T? ReadValue<T>(string sql) => Conn.ReadValue<T>(sql);




	// ******************
	// * Initialization *
	// ******************
	sealed record DBState(
		DateTime LastFetchTime,
		DateTime LastPullTime
	);

	void InitIFN()
	{
		if (conn_ != null) return;

		var now = DateTime.Now;
		var stateFile = Path.Combine(nfo.GetFolder(), "db-state.json");
		var (userFolder, folder) = (nfo.GetUserFolder(), nfo.GetFolder());


		// Clone the repo if it doesn't exist
		// ==================================
		if (!Directory.Exists(folder))
		{
			Cmd.RunSync(Consts.DoltExe, userFolder, ["clone", $"https://www.dolthub.com/repositories/{nfo.User}/{nfo.Name}"], true).EnsureSuccess();
			new DBState(now, now).Save(stateFile);
		}


		// Fetch & Pull if needed
		// ======================
		if (!File.Exists(stateFile))
		{
			Cmd.RunSync(Consts.DoltExe, folder, ["pull"]).EnsureSuccess();
			new DBState(now, now).Save(stateFile);
			LastPullTime = now;
		}
		else
		{
			var state = JsonUtils.Load<DBState>(stateFile);
			var (lastFetchTime, lastPullTime) = (state.LastFetchTime, state.LastPullTime);
			if (now - lastFetchTime > Consts.FetchDelay)
			{
				Cmd.RunSync(Consts.DoltExe, folder, ["fetch"]).EnsureSuccess();
				lastFetchTime = now;
				var pullNeeded = Cmd.RunSync(Consts.DoltExe, folder, ["pull"]).EnsureSuccess().StdOut.Contains("Your branch is behind");
				if (pullNeeded)
				{
					Cmd.RunSync(Consts.DoltExe, folder, ["pull"]).EnsureSuccess();
					lastPullTime = now;
				}
			}

			if (lastFetchTime != state.LastFetchTime || lastPullTime != state.LastPullTime)
				new DBState(lastFetchTime, lastPullTime).Save(stateFile);
			LastPullTime = lastPullTime;
		}


		// Start the server
		// ================
		Cmd.EnsureRunning(Consts.DoltExe, folder, ["sql-server", "-P", $"{nfo.Port}"]);


		// Open the connection
		// ===================
		Conn = new MySqlConnection($"Server=localhost;Port={nfo.Port};User ID=root;Database={nfo.Name}");
		Conn.Open();
	}
}



static class DoltDBExt
{
	public static T Load<T>(this DoltDB db, Keeper<T> keeper) => KeeperLogic.Load(db, keeper);
}