using System;
using System.Linq.Expressions;

namespace Ace
{
	public static class Expression
	{
		public static System.Linq.Expressions.Expression Unbox<TReturn>(this Expression<Func<TReturn>> expression) =>
			expression.Body is UnaryExpression unaryExpression
				? unaryExpression.Operand
				: expression.Body;
		
		public static string UnboxMemberName<TReturn>(this Expression<Func<TReturn>> expression) =>
			expression.Unbox().As<MemberExpression>()?.Member.Name ?? throw new ArgumentException(expression.ToString());
	}
}