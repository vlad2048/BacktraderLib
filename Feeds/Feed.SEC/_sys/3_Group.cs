using System.Collections.Concurrent;
using BaseUtils;
using Feed.SEC._sys.Rows;
using Feed.SEC._sys.Rows.Utils;
using Feed.SEC._sys.Utils;

namespace Feed.SEC._sys;

static class _3_Group
{
	public static void Run()
	{
		var Log = Logger.Make(LogCategory._3_Group);

		var quartersNext = Consts.Clean.GetAllQuarters();
		var quartersPrev = JsonUtils.LoadOr(Consts.Group.QuartersDoneFile, new List<string>());
		var quartersNew = quartersNext.ExceptA(quartersPrev);


		quartersNew.Loop(Log, 3, "Group", x => Consts.Clean.QuarterZipFile(x).FmtArchFile(), quarterNew =>
		{
			Chrono.Start("Read Clean.Rows", Log);
			var srcRows = RowReader.ReadRowSet(Consts.Clean.QuarterZipFile(quarterNew));


			Chrono.Start("Group per company");
			var srcSubs = srcRows.Subs.ToDictionary(e => e.Key.Adsh);
			var srcTags = srcRows.Tags.ToDictionary(e => e.Key);
			var srcNums = srcRows.Nums;
			var srcPres = srcRows.Pres;
			var dstSubsMap = srcSubs.Values.GroupInDict(e => e.Name);
			var dstNumsMap = srcNums.GroupInDict(e => srcSubs[e.Key.SubKey.Adsh].Name);
			var dstPresMap = srcPres.GroupInDict(e => srcSubs[e.Key.SubKey.Adsh].Name);
			var dstTagsForNumsMap = dstNumsMap.ToDictionary(kv => kv.Key, kv => kv.Value.Select(e => srcTags[e.Key.TagKey]));
			var dstTagsForPresMap = dstPresMap.ToDictionary(kv => kv.Key, kv => kv.Value.Select(e => srcTags[e.TagKey]));
			var dstTagsMap = Merge(dstTagsForNumsMap, dstTagsForPresMap);
			var companies = dstSubsMap.Keys.ToArray();


			Chrono.Start($"Write Group.Rows (threads:{Consts.Group.ParallelismLevel})");

			Parallelize.Run(
				companies,
				Consts.Group.ParallelismLevel,
				company =>
				{
					var dstSubs = dstSubsMap[company];
					var dstNums = dstNumsMap.GetValueOrDefault(company, []);
					var dstPres = dstPresMap.GetValueOrDefault(company, []);
					var dstTags = dstTagsMap.GetValueOrDefault(company, []);

					var dstArchFile = Consts.Group.CompanyZipFile(company);

					var rowSet = new RowSet(dstNums, dstPres, dstSubs, dstTags);

					RowReader.MergeRowSet(dstArchFile, rowSet);
				}
			);


			Chrono.Stop();
			quartersPrev.Add(quarterNew);
			quartersPrev.Save(Consts.Group.QuartersDoneFile);
		});
	}



	static class Parallelize
	{
		public static void Run<T>(T[] arr, int maxParallelism, Action<T> action)
		{
			var queue = MakeQueue(arr.Length);
			var tasks = Enumerable
				.Range(0, maxParallelism)
				.SelectA(_ => Task.Run(() =>
				{
					while (queue.TryDequeue(out var idx))
						action(arr[idx]);
				}));
			Task.WaitAll(tasks);
		}

		static ConcurrentQueue<int> MakeQueue(int count) => new(Enumerable.Range(0, count));
	}



	static IReadOnlyDictionary<K, V[]> Merge<K, V>(IReadOnlyDictionary<K, IEnumerable<V>> mapA, IReadOnlyDictionary<K, IEnumerable<V>> mapB) where K : notnull =>
		mapA.Keys.Concat(mapB.Keys).Distinct()
			.ToDictionary(
				e => e,
				e => mapA.GetValueOrDefault(e, []).ConcatDistinctA(mapB.GetValueOrDefault(e, []))
			);
}