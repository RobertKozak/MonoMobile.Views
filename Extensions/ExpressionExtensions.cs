//
// ExpressionExtensions.cs:
//
// Author:
//   Robert Kozak (rkozak@gmail.com)
//
// Copyright 2011, Nowcom Corporation
//
// Code licensed under the MIT X11 license
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
namespace System.Linq.Expressions
{
    using System.Collections.ObjectModel;
    using System.Linq;
	using System.Reflection;

    public static class ExpressionExtensions
    {
        public static string PropertyName<T>(this Expression<Func<T>> property)
        {
            if (property != null)
            {
                MemberExpression memberExp;
                if (!TryFindMemberExpression(property.Body, out memberExp))
                    return string.Empty;

                var memberNames = new Collection<string>();
                do
                {
                    memberNames.Add(memberExp.Member.Name);
                }
                while (TryFindMemberExpression(memberExp.Expression, out memberExp));

                var memberNamesArray = memberNames.Reverse().ToArray();

                var result = string.Join(".", memberNamesArray);
                return result;
            }

            return null;
        }
        
        private static bool TryFindMemberExpression(Expression expression, out MemberExpression memberExpression)
        {
            memberExpression = expression as MemberExpression;
            if (memberExpression != null)
                return true;

            // IsConversion checks for cases where the compiler created an automatic conversion, 
            // obj => Convert(obj.Property) [e.g., int -> object] 
            // OR: 
            // obj => ConvertChecked(obj.Property) [e.g., int -> long] 
            var unaryExpression = expression as UnaryExpression;
            if (IsConversion(expression) && (unaryExpression != null))
            {
                memberExpression = unaryExpression.Operand as MemberExpression;
                if (memberExpression != null)
                    return true;
            }

            return false;
        }

        private static bool IsConversion(Expression expression)
        {
            return (expression.NodeType == ExpressionType.Convert || expression.NodeType == ExpressionType.ConvertChecked);
        }
    }
    
    public static class AssignmentExpression
	{
        public static void SetValue<T>(Expression<Func<T>> expr, T value)
        {
            object obj;
            MemberInfo mInfo = expr.GetMemberInfo<T>(out obj);

            FieldInfo fInfo = mInfo as FieldInfo;
            PropertyInfo pInfo = mInfo as PropertyInfo;

            if (fInfo != null)
                fInfo.SetValue(obj, value);
            else if (pInfo != null && pInfo.CanWrite)
                pInfo.SetValue(obj, value, null);
        }

        public static MemberInfo GetMemberInfo<T>(this Expression<Func<T>> expr, out object obj)
        {
            if (expr == null)
                throw new ArgumentNullException("expr");

            var body = expr.Body as MemberExpression;

            if (body == null)
            {
                var unaryExpr = expr.Body as UnaryExpression;

                if (unaryExpr == null)
                    throw new ArgumentException("'expr' should be either an unary expression or a member expression");

                body = unaryExpr.Operand as MemberExpression;
            }

            if (body == null)
                throw new ArgumentException("'expr' should be a member expression");

            LambdaExpression lambdaExpr = Expression.Lambda(body.Expression);
            Delegate lambdaFunc = lambdaExpr.Compile();
            obj = lambdaFunc.DynamicInvoke();

            MemberInfo mInfo = obj.GetType().GetMember(body.Member.Name).FirstOrDefault();

            if (mInfo == null)
                throw new InvalidOperationException("Member not found.");

            return mInfo;
        }

	}
}
