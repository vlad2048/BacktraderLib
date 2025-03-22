using System.Reactive;

namespace BacktraderLib;

public interface IWhenChanged
{
	IObservable<Unit> WhenChanged { get; }
}

public interface IRoVar<out T> : IObservable<T>, IWhenChanged
{
	T V { get; }
}

public interface IHasDisp : IDisposable
{
	Disp D { get; }
}

public interface IRwVar<T> : IRoVar<T>, IHasDisp
{
	new T V { get; set; }
}