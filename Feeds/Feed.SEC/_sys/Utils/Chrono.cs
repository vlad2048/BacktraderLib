using System.Diagnostics;

namespace Feed.SEC._sys.Utils;



static class Chrono
{
	static Action<string>? Log;
	static Lap? lap;

	public static void Start(string name, Action<string>? Log_ = null)
	{
		Log ??= Log_;
		Stop();
		lap = new Lap(name);
	}

	public static void Stop()
	{
		lap?.Dispose();
		lap = null;
	}



	sealed class Lap : IDisposable
	{
		public void Dispose()
		{
			Log($"    {name} -> {watch.Elapsed.TotalSeconds:F3} s");
			watch.Stop();
		}

		readonly string name;
		readonly Stopwatch watch = Stopwatch.StartNew();

		public Lap(string name)
		{
			this.name = name;
			Log?.Invoke($"    {name} ...");
		}
	}
}