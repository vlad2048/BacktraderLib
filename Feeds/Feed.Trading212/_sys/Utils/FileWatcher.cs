using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Feed.Trading212._sys.Utils;

static class FileWatcher
{
	public static IObservable<Unit> Watch(string filename) =>
		Observable.Create<Unit>(obs =>
		{
			var obsD = new CompositeDisposable();
			var (folder, name) = filename.SplitFilename();
			var fsWatch = new FileSystemWatcher(folder, name)
			{
				NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName,
			}.D(obsD);

			Observable.Merge(
					fsWatch.WhenChanged().ToUnit(),
					fsWatch.WhenRenamed().Where(e => e.FullPath == filename).ToUnit()
				)
				.Throttle(Consts.ScriptDebounceTime)
				.Retry()
				.Subscribe(obs.OnNext).D(obsD);

			fsWatch.EnableRaisingEvents = true;

			return obsD;
		});


	public static T D<T>(this T obj, CompositeDisposable d) where T : IDisposable
	{
		d.Add(obj);
		return obj;
	}

	static (string, string) SplitFilename(this string file) => (
		Path.GetDirectoryName(file) ?? throw new ArgumentException(),
		Path.GetFileName(file)
	);

	static IObservable<FileSystemEventArgs> WhenChanged(this FileSystemWatcher watcher) => Observable.FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(e => watcher.Changed += e, e => watcher.Changed -= e).Select(e => e.EventArgs);
	static IObservable<RenamedEventArgs> WhenRenamed(this FileSystemWatcher watcher) => Observable.FromEventPattern<RenamedEventHandler, RenamedEventArgs>(e => watcher.Renamed += e, e => watcher.Renamed -= e).Select(e => e.EventArgs);
	static IObservable<Unit> ToUnit<T>(this IObservable<T> obs) => obs.Select(_ => Unit.Default);
}