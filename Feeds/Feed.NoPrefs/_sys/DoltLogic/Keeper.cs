using Feed.NoPrefs._sys.Utils;

namespace Feed.NoPrefs._sys.DoltLogic;



sealed record Keeper<T>(
	string File,
	Func<T> MakeEmpty,
	Func<DoltDB, T, T> Read
);



static class KeeperLogic
{
	sealed record KeeperData<T>(
		DateTime LastPullTime,
		T Data
	);

	
	public static T Load<T>(DoltDB db, Keeper<T> keeper)
	{
		if (!File.Exists(keeper.File))
		{
			var data = keeper.Read(db, keeper.MakeEmpty());
			new KeeperData<T>(db.LastPullTime, data).Save(keeper.File);
			return data;
		}
		else
		{
			var keeperData = JsonUtils.Load<KeeperData<T>>(keeper.File);
			if (keeperData.LastPullTime < db.LastPullTime)
			{
				var data = keeper.Read(db, keeperData.Data);
				new KeeperData<T>(db.LastPullTime, data).Save(keeper.File);
				return data;
			}
			else
			{
				return keeperData.Data;
			}
		}
	}
}