using System.Reactive.Linq;

namespace LINQPadPlus.Rx;


public sealed class RoVar<T> : IObservable<T>
{
	readonly IObservable<T> obs;
	internal RoVar(IObservable<T> obs) => this.obs = obs;

	public T V
	{
		get
		{
			T res = default!;
			obs.Take(1).Subscribe(e => res = e);
			return res;
		}
	}
	public IDisposable Subscribe(IObserver<T> observer) => obs.Subscribe(observer);
	
	public static implicit operator RoVar<T>(T value) => new(Obs.Return(value));
	public static implicit operator RoVar<T>(RwVar<T> rwVar) => new(rwVar);
}