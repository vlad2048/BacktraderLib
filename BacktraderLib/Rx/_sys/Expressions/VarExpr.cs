using System.Linq.Expressions;
using BacktraderLib._sys.Expressions.Utils;

namespace BacktraderLib._sys.Expressions;

static class VarExpr
{
	public static IRoVar<T> Expr<T>(Expression<Func<T>> expr, Disp d)
	{
		var vars = ExprUtils.PickVars(expr);
		var genLambda = ExprUtils.CreateGenLambda(expr, vars);
		object[] ParamsFun() => vars.Select(e => e.GetVal()).ToArray();

		var roVars = vars.Select(e => e.GetVar()).ToArray();
		var whenChangeArr = roVars.Select(Reflex.GetWhenVarChange).ToArray();
		var whenAnyChange = Obs.Merge(whenChangeArr);

		var varArr = ExprUtils.PrecompileLambda(genLambda);
		T Calc() => ExprUtils.EvalLambda<T>(varArr, ParamsFun);

		var resultVar = Var.Make(
			Calc(),
			whenAnyChange.Select(_ => Calc())
		);

		return resultVar;
	}
}