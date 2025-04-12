using System.Reactive;

namespace RxLib;

public static class RxExt
{
	public static IObservable<Unit> ToUnit<T>(this IObservable<T> obs) => obs.Select(_ => Unit.Default);
}