using BaseUtils;
using Feed.SEC._sys.Rows;
using Feed.SEC._sys.Rows.Utils;
using Feed.SEC._sys.Utils;
using FeedUtils;

namespace Feed.SEC._sys;

static class _4_Rename
{
	public static void Run()
	{
		var Log = Logger.Make(LogCategory._4_Rename);

		FileUtils.EmptyFolder(Consts.Rename.Folder);

		var subs = APIDev.Load_Clean_Rows<SubRow>().ToArray();
		var names = subs.ToHashSet(e => e.Name);
		var chgs = subs.GetChgs(names);
		var groups = chgs.FindGroups(names);

		var companies = Consts.Group.GetAllCompanies().ToHashSet();

		var usageMap = subs.CountBy(e => e.Name).ToDictionary(e => e.Key, e => e.Value);

		groups.Loop(Log, 3, "Rename", x => $"size:{x.Keys.Count}", group =>
		{
			if (group.Keys.Count == 1)
			{
				var currentName = group.Keys.First();
				File.Copy(Consts.Group.CompanyZipFile(currentName), Consts.Rename.CompanyZipFile(currentName));
			}
			else
			{
				var rowSet = group.Keys
					.Where(companies.Contains)
					.Select(e => RowReader.ReadRowSet(Consts.Group.CompanyZipFile(e)))
					.Merge();
				var currentName = (group.Changes.Length == 0) switch
				{
					true => group.Keys.Single(),
					false => GetCurrentName(group.Changes, usageMap),
				};

				RowReader.MergeRowSet(Consts.Rename.CompanyZipFile(currentName), rowSet);
			}
		});

		Log("Done");
	}


	sealed record Chg(string Name, string Former, DateOnly Changed);

	static Chg[] GetChgs(this SubRow[] subs, HashSet<string> names) =>
		subs
			.Where(e => e.Former != null && names.Contains(e.Former))
			.EnsureTrue(e => e.Changed != null, "Invalid data: Former and Changed should both be empty or both be non empty")
			.SelectDistinctA(e => new Chg(e.Name, e.Former!, e.Changed!.Value));


	static string GetCurrentName(Chg[] changes, IReadOnlyDictionary<string, int> usageMap)
	{
		var xs = changes
			.GroupBy(e => e.Changed)
			.OrderByDescending(e => e.Key)
			.First()
			.ToArray();

		return xs
			.OrderByDescending(e => usageMap[e.Name])
			.First()
			.Name;
	}



	sealed class Group
	{
		public HashSet<string> Keys { get; } = [];
		readonly List<Chg> changes;
		public Chg[] Changes => changes.OrderBy(e => e.Changed).ToArray();
		public Group(string name)
		{
			Keys.Add(name);
			changes = new List<Chg>();
		}
		Group(IEnumerable<string> keys, IEnumerable<Chg> changes) => (Keys, this.changes) = (keys.ToHashSet(), changes.ToList());
		public void AddChange(Chg chg) => changes.Add(chg);

		public Group Merge(Group other) => new(
			Keys.Concat(other.Keys),
			changes.Concat(other.changes)
		);
	}


	static Group[] FindGroups(this Chg[] chgs, HashSet<string> names)
	{
		var groups = names.Select(e => new Group(e)).ToList();
		foreach (var chg in chgs)
		{
			var idxPrev = groups.GetIdx(chg.Former);
			var idxNext = groups.GetIdx(chg.Name);
			if (idxPrev == idxNext)
			{
				groups[idxPrev].AddChange(chg);
			}
			else
			{
				var grpMerge = groups[idxPrev].Merge(groups[idxNext]);
				groups[idxPrev] = grpMerge;
				groups[idxPrev].AddChange(chg);
				groups.RemoveAt(idxNext);
			}
		}
		return [..groups];
	}



	static int GetIdx(this List<Group> groups, string name)
	{
		for (var i = 0; i < groups.Count; i++)
			if (groups[i].Keys.Contains(name))
				return i;
		throw new ArgumentException("Impossible. Could not find group");
	}
}