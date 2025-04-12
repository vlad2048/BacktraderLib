using System.Text.Json;

namespace FeedUtils;

public sealed class FetchLimiter(string stateFile, TimeSpan delay)
{
	public bool IsFetchNeeded()
	{
		var timeNext = DateTime.Now;
		var timePrev = File.Exists(stateFile) switch
		{
			false => DateTime.MinValue,
			true => JsonSerializer.Deserialize<DateTime>(File.ReadAllText(stateFile)),
		};
		return timeNext - timePrev >= delay;
	}

	public void ConfirmFetchDone() => File.WriteAllText(stateFile, JsonSerializer.Serialize(DateTime.Now));
}