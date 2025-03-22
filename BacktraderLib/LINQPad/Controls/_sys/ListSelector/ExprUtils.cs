using System.Linq.Expressions;

namespace BacktraderLib._sys.ListSelector;

static class ExprUtils
{
	public static (string, Func<T, object>) GetNameAndGetter<T>(Expression<Func<T, object>> expr) =>
	(
		GetName(expr),
		expr.Compile()
	);

	static string GetName<T>(Expression<Func<T, object>> expr)
	{
		var lambda = (LambdaExpression)expr;
		var p = lambda.Parameters.Single();

		var visitor = new NameVisitor(p);
		visitor.Visit(expr);

		if (visitor.Name == null) throw new ArgumentException("Expression name not found");

		return visitor.Name;
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