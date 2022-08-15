using System;
using System.Linq.Expressions;

namespace Ace
{
	public static class Expression_
	{
		public static string UnboxMemberName(this LambdaExpression lambdaExpression) =>
			lambdaExpression.Body.Unbox() is MemberExpression memberExpression
				? memberExpression.Member.Name
				: throw new ArgumentException(lambdaExpression.ToString());

		private static Expression Unbox(this Expression expression) =>
			expression is UnaryExpression unaryExpression
				? unaryExpression.Operand
				: expression;
	}
}