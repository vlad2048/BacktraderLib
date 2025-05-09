using System.Reactive.Subjects;

namespace LINQPadPlus.Rx;

public sealed class RwVar<T> : IObservable<T>
{
	readonly BehaviorSubject<T> subj;
	readonly bool isReadOnly;
	internal RwVar(T value, bool isReadOnly) => (subj, this.isReadOnly) = (new BehaviorSubject<T>(value), isReadOnly);
	
	public override string ToString() => $"RwVar({V})";
	public T V
	{
		get => subj.Value;
		set
		{
			if (isReadOnly) throw new ArgumentException("This Variable is actually ReadOnly");
			if (value != null && value.Equals(subj.Value)) return;
			subj.OnNext(value);
		}
	}
	public IDisposable Subscribe(IObserver<T> observer) => subj.Subscribe(observer);
	
	public static implicit operator RwVar<T>(T value) => new(value, true);
}