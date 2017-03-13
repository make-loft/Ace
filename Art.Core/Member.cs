using System;
using System.Linq.Expressions;

namespace Aero
{
    public static class Member
    {
        public static string ExtractName<TReturn>(this Expression<Func<TReturn>> expression)
        {
            var unaryExpression = expression.Body as UnaryExpression;
            var memberExpression = unaryExpression == null
                ? expression.Body as MemberExpression
                : unaryExpression.Operand as MemberExpression;

            return memberExpression?.Member.Name ?? throw new ArgumentException(expression.ToString());
        }
    }
}