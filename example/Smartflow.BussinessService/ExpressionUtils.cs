using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Smartflow.BussinessService
{
    public enum ConstantCharacter
    {
        None,
        /// <summary>
        /// 模糊
        /// </summary>
        Like,
        /// <summary>
        /// 等于=
        /// </summary>
        Equals,
        /// <summary>
        /// 左边表达式
        /// </summary>
        Left,
        /// <summary>
        /// 右边表达式
        /// </summary>
        Right
    }

    public class ExpressionUtils
    {
        public static string ParseToWhere(Expression expression)
        {
            BinaryExpression body = (expression as BinaryExpression);
            StringBuilder buildWhere = new StringBuilder();
            ParseBinaryExpression(body, buildWhere);
            return string.IsNullOrEmpty(buildWhere.ToString()) ? "" : " And " + buildWhere.ToString();
        }

        //public static string GeneratorCondition(Expression<Func<T, bool>> exp)
        //{
        //    BinaryExpression body = (exp.Body as BinaryExpression);
        //    StringBuilder buildWhere = new StringBuilder();
        //    ParseBinaryExpression(body, buildWhere);
        //    return buildWhere.ToString();
        //}

        private static void ParseBinaryExpression(BinaryExpression expr, StringBuilder builder)
        {
            if (expr != null)
            {
                string character = Operator(expr.NodeType);
                if (character.ToLower() == "or")
                {
                    builder.AppendFormat("  ( ");
                }
                if (expr.Left != null)
                {
                    ParseBinaryExpression(expr.Left as BinaryExpression, builder);
                    ParseMethodCall(expr.Left, builder);
                    ParseMember(expr.Left as MemberExpression, builder, ConstantCharacter.Left);
                    ParseConstant(expr.Left as ConstantExpression, builder, ConstantCharacter.Equals);
                }

                builder.Append(character);


                if (expr.Right != null)
                {
                    ParseBinaryExpression(expr.Right as BinaryExpression, builder);
                    ParseMethodCall(expr.Right, builder);
                    ParseMember(expr.Right as MemberExpression, builder, ConstantCharacter.Right);
                    ParseConstant(expr.Right as ConstantExpression, builder, ConstantCharacter.Equals);
                }

                if (character.ToLower() == "or")
                {
                    builder.AppendFormat("  ) ");
                }
            }
        }

        private static void ParseMethodCall(Expression expr, StringBuilder builder)
        {
            if (expr is MethodCallExpression)
            {
                MethodCallExpression method = expr as MethodCallExpression;
                
                string methodName = method.Method.Name;
                switch (methodName)
                {
                    case "Contains":
                        ParseMember(method.Object, builder, ConstantCharacter.None);
                        builder.Append(" LIKE ");
                        ParseConstant(method.Arguments[0], builder, ConstantCharacter.Like);
                        break;
                    case "ToString":

                        string value= Expression.Lambda(method, null).Compile().DynamicInvoke().ToString();
                        builder.AppendFormat("'{0}'", value);
                        //ParseConstant(method.Arguments[0], builder, ConstantCharacter.Like);
                        break;
                    default:
                        break;
                }

            }
        }

        private static void ParseMember(Expression expr, StringBuilder builder, ConstantCharacter character)
        {
            if (expr is MemberExpression)
            {
                MemberExpression member = expr as MemberExpression;
                if (member.Member.MemberType == System.Reflection.MemberTypes.Field)
                {
                    builder.AppendFormat(" '{0}' ", (member.Member as FieldInfo).GetValue((member.Expression as ConstantExpression).Value));
                }
                else if (member.Member.MemberType == System.Reflection.MemberTypes.Property && character == ConstantCharacter.Right)
                {
                    PropertyInfo info = ((PropertyInfo)member.Member);
                    PropertyInfo outerProp = (PropertyInfo)member.Member;
                    MemberExpression innerMember = (MemberExpression)member.Expression;
                    FieldInfo innerField = (FieldInfo)innerMember.Member;
                    ConstantExpression ce = (ConstantExpression)innerMember.Expression;
                    object innerObj = ce.Value;
                    object outerObj = innerField.GetValue(innerObj);
                    string value = (string)outerProp.GetValue(outerObj, null);
                    builder.AppendFormat(" '{0}' ", value);
                }
                else
                {
                    builder.AppendFormat(" {0} ", member.Member.Name);
                }
            }
        }

        private static void ParseConstant(Expression expr, StringBuilder builder, ConstantCharacter character)
        {
            if (expr is ConstantExpression)
            {
                ConstantExpression constant = expr as ConstantExpression;
                builder.AppendFormat("'{0}'",
                    (character == ConstantCharacter.Like) ?
                    "%" + constant.Value.ToString() + "%" :
                    constant.Value.ToString());
            }
        }

        private static string Operator(ExpressionType nodeType)
        {
            string character = string.Empty;
            switch (nodeType)
            {
                case ExpressionType.Add:
                    break;
                case ExpressionType.AddAssign:
                    break;
                case ExpressionType.AddAssignChecked:
                    break;
                case ExpressionType.AddChecked:
                    break;
                case ExpressionType.And:
                    break;
                case ExpressionType.Call:
                case ExpressionType.AndAlso:
                    character = "and";
                    break;
                case ExpressionType.AndAssign:
                    break;
                case ExpressionType.ArrayIndex:
                    break;
                case ExpressionType.ArrayLength:
                    break;
                case ExpressionType.Assign:
                    break;
                case ExpressionType.Block:
                    break;
                case ExpressionType.Coalesce:
                    break;
                case ExpressionType.Conditional:
                    break;
                case ExpressionType.Constant:
                    break;
                case ExpressionType.Convert:
                    break;
                case ExpressionType.ConvertChecked:
                    break;
                case ExpressionType.DebugInfo:
                    break;
                case ExpressionType.Decrement:
                    break;
                case ExpressionType.Default:
                    break;
                case ExpressionType.Divide:
                    break;
                case ExpressionType.DivideAssign:
                    break;
                case ExpressionType.Dynamic:
                    break;
                case ExpressionType.Equal:
                    character = "=";
                    break;
                case ExpressionType.ExclusiveOr:
                    break;
                case ExpressionType.ExclusiveOrAssign:
                    break;
                case ExpressionType.Extension:
                    break;
                case ExpressionType.Goto:
                    break;
                case ExpressionType.GreaterThan:
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    break;
                case ExpressionType.Increment:
                    break;
                case ExpressionType.Index:
                    break;
                case ExpressionType.Invoke:
                    break;
                case ExpressionType.IsFalse:
                    break;
                case ExpressionType.IsTrue:
                    break;
                case ExpressionType.Label:
                    break;
                case ExpressionType.Lambda:
                    break;
                case ExpressionType.LeftShift:
                    break;
                case ExpressionType.LeftShiftAssign:
                    break;
                case ExpressionType.LessThan:
                    break;
                case ExpressionType.LessThanOrEqual:
                    break;
                case ExpressionType.ListInit:
                    break;
                case ExpressionType.Loop:
                    break;
                case ExpressionType.MemberAccess:
                    break;
                case ExpressionType.MemberInit:
                    break;
                case ExpressionType.Modulo:
                    break;
                case ExpressionType.ModuloAssign:
                    break;
                case ExpressionType.Multiply:
                    break;
                case ExpressionType.MultiplyAssign:
                    break;
                case ExpressionType.MultiplyAssignChecked:
                    break;
                case ExpressionType.MultiplyChecked:
                    break;
                case ExpressionType.Negate:
                    break;
                case ExpressionType.NegateChecked:
                    break;
                case ExpressionType.New:
                    break;
                case ExpressionType.NewArrayBounds:
                    break;
                case ExpressionType.NewArrayInit:
                    break;
                case ExpressionType.Not:
                    break;
                case ExpressionType.NotEqual:
                    break;
                case ExpressionType.OnesComplement:
                    break;
                case ExpressionType.Or:
                    break;
                case ExpressionType.OrAssign:
                    break;
                case ExpressionType.OrElse:
                    character = "OR";
                    break;
                case ExpressionType.Parameter:
                    break;
                case ExpressionType.PostDecrementAssign:
                    break;
                case ExpressionType.PostIncrementAssign:
                    break;
                case ExpressionType.Power:
                    break;
                case ExpressionType.PowerAssign:
                    break;
                case ExpressionType.PreDecrementAssign:
                    break;
                case ExpressionType.PreIncrementAssign:
                    break;
                case ExpressionType.Quote:
                    break;
                case ExpressionType.RightShift:
                    break;
                case ExpressionType.RightShiftAssign:
                    break;
                case ExpressionType.RuntimeVariables:
                    break;
                case ExpressionType.Subtract:
                    break;
                case ExpressionType.SubtractAssign:
                    break;
                case ExpressionType.SubtractAssignChecked:
                    break;
                case ExpressionType.SubtractChecked:
                    break;
                case ExpressionType.Switch:
                    break;
                case ExpressionType.Throw:
                    break;
                case ExpressionType.Try:
                    break;
                case ExpressionType.TypeAs:
                    break;
                case ExpressionType.TypeEqual:
                    break;
                case ExpressionType.TypeIs:
                    break;
                case ExpressionType.UnaryPlus:
                    break;
                case ExpressionType.Unbox:
                    break;
                default:
                    break;
            }
            return character;
        }
    }
}
