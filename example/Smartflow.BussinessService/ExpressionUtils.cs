using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Smartflow.BussinessService
{
    public class ExpressionUtils
    {
        /*
         * 简单的条件生成器
         */
        public static void ParseExpressionToWhere(BinaryExpression binaryExpression, StringBuilder sb)
        {
            switch (binaryExpression.NodeType)
            {
                case ExpressionType.Equal:
                    MemberExpression memberExpression = (binaryExpression.Left as MemberExpression);
                    string propertyName = memberExpression.Member.Name;
                    ConstantExpression constantExpression = (binaryExpression.Right as ConstantExpression);
                    string rightValue = string.Empty;
                    if (constantExpression == null)
                    {
                        MemberExpression mexpr = (binaryExpression.Right as MemberExpression);
                        rightValue = Expression.Lambda(mexpr).Compile().DynamicInvoke().ToString();
                    }
                    else
                    {
                        rightValue = constantExpression.Value.ToString();
                    }
                    sb.AppendFormat(" {0}='{1}' ", propertyName, rightValue);
                    break;
                case ExpressionType.OrElse:
                    sb.Append("(");
                    BinaryExpression OLeftExpression = (binaryExpression.Left as BinaryExpression);
                    ParseExpressionToWhere(OLeftExpression, sb);
                    sb.Append("OR");
                    BinaryExpression ORigthExpression = (binaryExpression.Right as BinaryExpression);
                    ParseExpressionToWhere(ORigthExpression, sb);
                    sb.Append(")");
                    break;
                case ExpressionType.AndAlso:
                    BinaryExpression ALeftExpression = (binaryExpression.Left as BinaryExpression);
                    ParseExpressionToWhere(ALeftExpression, sb);
                    sb.Append("AND");
                    BinaryExpression ARigthExpression = (binaryExpression.Right as BinaryExpression);
                    ParseExpressionToWhere(ARigthExpression, sb);
                    break;
                default:
                    break;
            }
        }

        public static string ParseToWhere<T>(Expression expression)
        {
            StringBuilder whereBuilder = new StringBuilder();
            ExpressionUtils.ParseExpressionToWhere(expression as BinaryExpression, whereBuilder);
            return (!String.IsNullOrEmpty(whereBuilder.ToString())) ? "AND " + whereBuilder.ToString() : "";
        }
    }
}
