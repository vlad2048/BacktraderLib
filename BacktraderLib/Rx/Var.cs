using BacktraderLib._sys.Expressions;
using System.Linq.Expressions;
using System.Reactive;

namespace BacktraderLib;

public static class Var
{
	public static IRwVar<T> Make<T>(T init) => new RwVar<T>(init, D);
	public static IRoVar<T> Make<T>(T init, IObservable<T> obs)
	{
		var rxVar = new RwVar<T>(init, D);
		obs.Subscribe(v => rxVar.V = v).D(D);
		return rxVar;
	}

	public static IRoVar<T> MakeConst<T>(T val) => Obs.Return(val).ToVarInner();



	public static IRoVar<T> ToVar<T>(this IObservable<T> obs) => new RoVar<T>(obs.MakeReplay());


	public static IObservable<T> MakeReplay<T>(this IObservable<T> src)
	{
		var srcConn = src.Replay(1);
		srcConn.Connect().D(D);
		return srcConn;
	}

	public static IObservable<T> MakeHot<T>(this IObservable<T> src)
	{
		var srcConn = src.Publish();
		srcConn.Connect().D(D);
		return srcConn;
	}

	public static IObservable<Unit> Merge(params IWhenChanged[] srcs) => srcs.Select(e => e.WhenChanged).Merge();

	public static IRoVar<U> SelectVar<T, U>(this IRoVar<T> rx, Func<T, U> fun) =>
		Make(
			fun(rx.V),
			rx.Select(fun)
		);

	public static IRoVar<U> Switch<T, U>(this IRoVar<T> rx, Func<T, IRoVar<U>> sel) =>
		Make(
			sel(rx.V).V,
			rx.Select(sel).Switch()
		);

	public static IRoVar<T> Expr<T>(Expression<Func<T>> expr) => VarExpr.Expr(expr, D);












	static IRoVar<T> ToVarInner<T>(this IObservable<T> obs) => new RoVar<T>(obs.Replay(1).RefCount());


	sealed class RoVar<T>(IObservable<T> obs) : IRoVar<T>
	{
		public IDisposable Subscribe(IObserver<T> observer) => obs.Subscribe(observer);
		public T V => Task.Run(async () => await obs.FirstAsync()).Result;

		public IObservable<Unit> WhenChanged => this.ToUnit();

		public override string ToString() => $"RoVar({V})";
	}


	sealed class RwVar<T>(T init, Disp d) : IRwVar<T>
	{
		readonly BehaviorSubject<T> subj = new BehaviorSubject<T>(init).D(d);

		public Disp D { get; } = d;
		public void Dispose() => D.Dispose();
		public IDisposable Subscribe(IObserver<T> observer) => subj.Subscribe(observer);

		public T V
		{
			get => subj.Value;
			set
			{
				if (value != null && value.Equals(subj.Value)) return;
				subj.OnNext(value);
			}
		}

		public IObservable<Unit> WhenChanged => this.ToUnit();

		public override string ToString() => $"RwVar({V})";
	}
}