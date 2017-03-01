using System;
using System.Linq.Expressions;

namespace Aero
{
    public class Member
    {
        public static string ExtractName<TReturn>(Expression<Func<TReturn>> expression)
        {
            var unaryExpression = expression.Body as UnaryExpression;
            var memberExpression = unaryExpression == null
                ? expression.Body as MemberExpression
                : unaryExpression.Operand as MemberExpression;

            if (memberExpression == null)
                throw new ArgumentException(expression.ToString());

            return memberExpression.Member.Name;
        }
    }
}