using System.Linq.Expressions;
using System.Reflection;

namespace VmsDataApi.Utils
{
    public static class ExpressionEx
    {
        public static Expression<Func<T, TResult>> And<T, TResult>(this Expression<Func<T, TResult>> expr1, Expression<Func<T, TResult>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, TResult>>(Expression.AndAlso(expr1.Body, invokedExpr), expr1.Parameters);
        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>(Expression.OrElse(expr1.Body, invokedExpr), expr1.Parameters);
        }

        public static Expression<Func<T, bool>> GetContainsExpression<T>(string propertyName, string propertyValue)
        {
            var parameterExp = Expression.Parameter(typeof(T), "type");
            var propertyExp = Expression.Property(parameterExp, propertyName);
            MethodInfo method = typeof(string).GetMethod("Contains", [typeof(string)]);
            var someValue = Expression.Constant(propertyValue, typeof(string));
            var containsMethodExp = Expression.Call(propertyExp, method, someValue);

            return Expression.Lambda<Func<T, bool>>(containsMethodExp, parameterExp);
        }

        public static Expression<Func<T, bool>> ODataFiletrToLinqExpression<T>(string odataFilterString)
        {
            if (odataFilterString.Contains(" and "))
            {
                var andIndex = odataFilterString.IndexOf(" and ");
                var leftExpression = ODataFiletrToLinqExpression<T>(odataFilterString.Substring(0, andIndex));
                var rightExpression = ODataFiletrToLinqExpression<T>(odataFilterString.Substring(andIndex + 5));
                return leftExpression.And(rightExpression);

            }
            else if (odataFilterString.Contains(" or "))
            {
                var andIndex = odataFilterString.IndexOf(" or ");
                var leftExpression = ODataFiletrToLinqExpression<T>(odataFilterString.Substring(0, andIndex));
                var rightExpression = ODataFiletrToLinqExpression<T>(odataFilterString.Substring(andIndex + 5));
                return leftExpression.Or(rightExpression);
            }
            else if (odataFilterString.Contains("contains"))
            {
                var openBracketIndex = odataFilterString.IndexOf("(");
                var closeBracketIndex = odataFilterString.IndexOf(")");
                var propAndConst = odataFilterString.Substring(openBracketIndex + 1, closeBracketIndex - openBracketIndex - 1);
                var propName = propAndConst.Split(',')[0];
                var propValue = propAndConst.Split(',')[1].Trim('\'');
                return GetContainsExpression<T>(propName, propValue);
            }


            //if(odataFilterString)

            var parts = odataFilterString.Split(' ');
            if (parts.Length != 3)
            {
                throw new ArgumentException("Invalid OData filter string format.");
            }

            var propertyName = parts[0];
            var operatorString = parts[1];
            var operatorExpression = GetOperatorExpression(operatorString);
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, propertyName);

            object? value = parts[2].GetValueAccordingToMemberType(property.Member);
            var constant = Expression.Constant(value);
            var body = operatorExpression(property, constant);
            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }

        private static object? GetValueAccordingToMemberType(this string value, MemberInfo memberInfo)
        {
            var memberInfoStr = memberInfo.ToString() ?? string.Empty;
            if (memberInfoStr.StartsWith("Int32"))
            {
                return int.Parse(value);
            }
            else if (memberInfoStr.StartsWith("Int64"))
            {
                return long.Parse(value);
            }
            else
            {
                return value.Trim('\'');
            }
        }

        private static Func<Expression, ConstantExpression, BinaryExpression> GetOperatorExpression(string operatorString)
        {
            switch (operatorString.ToLower())
            {
                case "eq":
                    return Expression.Equal;
                case "ne":
                    return Expression.NotEqual;
                case "gt":
                    return Expression.GreaterThan;
                case "ge":
                    return Expression.GreaterThanOrEqual;
                case "lt":
                    return Expression.LessThan;
                case "le":
                    return Expression.LessThanOrEqual;
                default:
                    throw new ArgumentException($"Unsupported operator: {operatorString}");
            }
        }
    }
}
