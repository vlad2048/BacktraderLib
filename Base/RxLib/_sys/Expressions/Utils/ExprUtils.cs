using System.Linq.Expressions;
using RxLib._sys.Expressions.Structs;
using RxLib._sys.Expressions.Visitors;

namespace RxLib._sys.Expressions.Utils;

static class ExprUtils
{
	public static VarNfo[] PickVars<T>(Expression<Func<T>> expr)
	{
		var visitor = new VarPickerExprVisitor();
		visitor.Visit(expr);
		return visitor.Vars;
	}

	public static LambdaExpression CreateGenLambda<T>(Expression<Func<T>> expr, VarNfo[] vars)
	{
		var visitor = new GenLambdaExprVisitor(vars);
		var genExpr = visitor.Visit(expr);
		if (genExpr is not LambdaExpression lambdaGenExpr) throw new ArgumentException();
		return lambdaGenExpr;
	}

	public static Delegate PrecompileLambda(LambdaExpression lambda)
	{
		var deleg = lambda.Compile(true);
		return deleg;
	}

	public static T EvalLambda<T>(Delegate deleg, Func<object[]> paramsFun)
	{
		var resultUntyped = deleg.DynamicInvoke(paramsFun());
		if (resultUntyped is not T result) throw new ArgumentException();
		return result;
	}
}