namespace Feed.Yahoo._sys.Utils;

static class TimeUtils
{
	public static long ToUnixTime(this DateTime e) => (long)(e - DateTime.UnixEpoch).TotalSeconds;

	public static DateTime ToDateTime(this long e) => DateTime.UnixEpoch.AddSeconds(e);
}