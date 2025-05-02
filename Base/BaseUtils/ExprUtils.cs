using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace BaseUtils;

public static class ExprUtils
{
	/*

	Expression<Func<T, V>>
	======================
		Type																			NodeType
		-----------------------------------------------------------------------------------------
		MemberExpression (private: PropertyExpression)									MemberAccess
			Expression=ParameterExpression (private: TypedParameterExpression)			Parameter


	Expression<Func<T, object>>
	===========================
		Type																			NodeType
		--------------------------------------------------------------------------------------------
			UnaryExpression																Convert
				Operand=MemberExpression (private: PropertyExpression)					MemberAccess
					Expression=ParameterExpression (private: TypedParameterExpression)	Parameter

	*/
	public static bool IsSimpleAccessor<T, V>(this Expression<Func<T, V>> expr)
	{
		if (
			expr.Body.Is<MemberExpression>(ExpressionType.MemberAccess, out var exprA) &&
			exprA.Expression != null &&
			exprA.Expression.Is<ParameterExpression>(ExpressionType.Parameter, out _)
		)
			return true;

		if (
			expr.Body.Is<UnaryExpression>(ExpressionType.Convert, out var exprB) &&
			exprB.Operand.Is<MemberExpression>(ExpressionType.MemberAccess, out var exprB2) &&
			exprB2.Expression != null &&
			exprB2.Expression.Is<ParameterExpression>(ExpressionType.Parameter, out _)
		)
			return true;

		return false;
	}



	public static string GetSimpleAccessorName<T>(this Expression<Func<T, object>> expr)
	{
		if (!expr.IsSimpleAccessor()) throw new ArgumentException("Expression must be a simple property accessor");

		var lambda = (LambdaExpression)expr;
		var p = lambda.Parameters.Single();

		var visitor = new NameVisitor(p);
		visitor.Visit(expr);

		if (visitor.Name == null) throw new ArgumentException("Expression name not found");

		return visitor.Name;
	}




	public static Type GetSimpleAccessorPropertyType<T>(this Expression<Func<T, object>> expr)
	{
		if (!expr.IsSimpleAccessor()) throw new ArgumentException("Expression must be a simple property accessor");

		if (
			expr.Body.Is<MemberExpression>(ExpressionType.MemberAccess, out var exprA)
		)
			return exprA.Type;

		if (
			expr.Body.Is<UnaryExpression>(ExpressionType.Convert, out var exprB) &&
			exprB.Operand.Is<MemberExpression>(ExpressionType.MemberAccess, out var exprB2)
		)
			return exprB2.Type;

		throw new ArgumentException("Failed to get property type for simple accessor");
	}




	static bool Is<T>(this Expression expr, ExpressionType exprType, [NotNullWhen(true)] out T? typedExpr) where T : Expression
	{
		if (expr is T typedExpr_ && expr.NodeType == exprType)
		{
			typedExpr = typedExpr_;
			return true;
		}
		else
		{
			typedExpr = null;
			return false;
		}
	}


	sealed class NameVisitor(ParameterExpression p) : ExpressionVisitor
	{
		public string? Name { get; private set; }
		protected override Expression VisitMember(MemberExpression node)
		{
			if (node.Expression == p)
				Name ??= node.Member.Name;
			return base.VisitMember(node);
		}
	}
}