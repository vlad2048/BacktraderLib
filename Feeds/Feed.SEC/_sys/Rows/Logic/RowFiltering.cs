using BaseUtils;
using Feed.SEC._sys.Rows.Base;

namespace Feed.SEC._sys.Rows.Logic;

static class RowFiltering
{
	public static RowSet Tidyup(this RowSet set, bool disableCheck = false)
	{
		var (nums, pres, subs, tags) = set;

		var subKeys = subs.Select(e => e.Key).ToHashSet();
		var tagKeys = tags.Select(e => e.Key).ToHashSet();

		nums = ForeignKeyViolation_RemoveSrcRows<NumRow, SubRow, SubRowKey>(nums, e => e.Key.SubKey, subKeys);
		nums = ForeignKeyViolation_RemoveSrcRows<NumRow, TagRow, TagRowKey>(nums, e => e.Key.TagKey, tagKeys);
		pres = ForeignKeyViolation_RemoveSrcRows<PreRow, SubRow, SubRowKey>(pres, e => e.Key.SubKey, subKeys);
		pres = ForeignKeyViolation_RemoveSrcRows<PreRow, TagRow, TagRowKey>(pres, e => e.TagKey, tagKeys);

		var numsSubKeys = nums.Select(e => e.Key.SubKey).Distinct().ToHashSet();
		subs = subs.WhereA(e => numsSubKeys.Contains(e.Key));

//		pres = pres.WhereA(e => numTagKeys.Contains(e.TagKey));

		pres = PartialKeyViolation_RemoveSrcRows<PreRow, NumRow, (SubRowKey, TagRowKey)>(pres, e => (e.Key.SubKey, e.TagKey), nums, e => (e.Key.SubKey, e.Key.TagKey));

		var numTagKeys = nums.Select(e => e.Key.TagKey).ToHashSet();
		tags = tags.WhereA(e => numTagKeys.Contains(e.Key));

		var res = new RowSet(
			nums,
			pres,
			subs,
			tags
		);
		if (!disableCheck)
			res.EnsureConsistent();
		return res;
	}



	static TSrc[] ForeignKeyViolation_RemoveSrcRows<TSrc, TDst, TDstK>(TSrc[] srcRows, Func<TSrc, TDstK> refFun, HashSet<TDstK> dstKeys) where TSrc : IRow<TSrc> where TDst : IRow<TDst> =>
		srcRows
			.WhereA(e => dstKeys.Contains(refFun(e)));



	static TSrc[] PartialKeyViolation_RemoveSrcRows<TSrc, TDst, PartialK>(TSrc[] srcRows, Func<TSrc, PartialK> srcFun, TDst[] dstRows, Func<TDst, PartialK> dstFun) where TSrc : IRow<TSrc> where TDst : IRow<TDst>
	{
		var dstKeys = dstRows.Select(dstFun).ToHashSet();
		return srcRows.WhereA(e => dstKeys.Contains(srcFun(e)));
	}
}