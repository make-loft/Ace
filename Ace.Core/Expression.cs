using System;
using System.Linq.Expressions;

namespace Ace
{
	public static class Expression
	{
		public static MemberExpression UnboxExpression<TReturn>(this Expression<Func<TReturn>> expression) =>
			expression.Body is UnaryExpression unaryExpression
				? unaryExpression.Operand as MemberExpression
				: expression.Body as MemberExpression;
		
		public static string UnboxMemberName<TReturn>(this Expression<Func<TReturn>> expression) => 
			UnboxExpression(expression)?.Member.Name ?? throw new ArgumentException(expression.ToString());
	}
}