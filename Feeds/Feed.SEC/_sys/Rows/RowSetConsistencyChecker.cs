using Feed.SEC._sys.Utils;

namespace Feed.SEC._sys.Rows;

static class RowSetConsistencyChecker
{
	public static RowSet CheckConsistency(this RowSet set, string? archFile = null)
	{
		string Err(string msg) => $"{msg} in {archFile ?? "n/a"}";

		var (nums, pres, subs, tags) = set;

		// ReSharper disable UnusedVariable
		var subKeys = subs.EnsureUnique(e => e.Key, Err("Duplicate rows in [Sub]"));
		var numKeys = nums.EnsureUnique(e => e.Key, Err("Duplicate rows in [Num]"));
		var tagKeys = tags.EnsureUnique(e => e.Key, Err("Duplicate rows in [Tag]"));
		var preKeys = pres.EnsureUnique(e => e.Key, Err("Duplicate rows in [Pre]"));
		// ReSharper restore UnusedVariable

		var numPartialKeys = nums.Select(e => (AdshKey: e.Key.SubKey, TagVersionKey: e.Key.TagKey)).Distinct().ToHashSet();

		nums.EnsureRefExists(e => e.Key.SubKey, subKeys, Err("[Num] with no [Sub]"));
		nums.EnsureRefExists(e => e.Key.TagKey, tagKeys, Err("[Num] with no [Tag]"));
		pres.EnsureRefExists(e => e.Key.SubKey, subKeys, Err("[Pre] with no [Sub]"));
		pres.EnsureRefExists(e => e.TagKey, tagKeys, Err("[Pre] with no [Tag]"));

		pres.EnsureRefExists(e => (AdshKey: e.Key.SubKey, TagVersionKey: e.TagKey), numPartialKeys, Err("[Pre] with no [Sub],[Tag]"));

		return set;
	}
}