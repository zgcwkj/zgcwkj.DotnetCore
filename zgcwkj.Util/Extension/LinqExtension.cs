using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace zgcwkj.Util.Extension
{
    /// <summary>
    /// Linq 拓展
    /// </summary>
    public static class LinqExtension
    {
        /// <summary>
        /// 属性
        /// </summary>
        /// <param name="expression">Linq</param>
        /// <param name="propertyName">属性名</param>
        /// <returns></returns>
        public static Expression Property(this Expression expression, string propertyName)
        {
            return Expression.Property(expression, propertyName);
        }

        /// <summary>
        /// 合并 Linq
        /// </summary>
        /// <param name="left">左侧 Linq</param>
        /// <param name="right">右侧 Linq</param>
        /// <returns></returns>
        public static Expression AndAlso(this Expression left, Expression right)
        {
            return Expression.AndAlso(left, right);
        }

        /// <summary>
        /// 使用参数的方法的调用
        /// </summary>
        /// <param name="instance">实例</param>
        /// <param name="methodName">方法名称</param>
        /// <param name="arguments">参数</param>
        /// <returns></returns>
        public static Expression Call(this Expression instance, string methodName, params Expression[] arguments)
        {
            return Expression.Call(instance, instance.Type.GetMethod(methodName), arguments);
        }

        /// <summary>
        /// 它表示 大于 数字比较
        /// </summary>
        /// <param name="left">左侧 Linq</param>
        /// <param name="right">右侧 Linq</param>
        /// <returns></returns>
        public static Expression GreaterThan(this Expression left, Expression right)
        {
            return Expression.GreaterThan(left, right);
        }

        /// <summary>
        /// ToLambda
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="body">Linq</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public static Expression<T> ToLambda<T>(this Expression body, params ParameterExpression[] parameters)
        {
            return Expression.Lambda<T>(body, parameters);
        }

        /// <summary>
        /// 默认条件为真
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> True<T>() { return param => true; }

        /// <summary>
        /// 默认条件为假
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> False<T>() { return param => false; }

        /// <summary>
        /// 条件组合 And
        /// </summary>
        /// <returns></returns>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.AndAlso);
        }

        /// <summary>
        /// 条件组合 Or
        /// </summary>
        /// <returns></returns>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.OrElse);
        }

        /// <summary>
        /// 使用指定的合并功能将第一个表达式与第二个表达式组合
        /// Combines the first expression with the second using the specified merge function.
        /// </summary>
        static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
        {
            var map = first.Parameters
                .Select((f, i) => new { f, s = second.Parameters[i] })
                .ToDictionary(p => p.s, p => p.f);
            var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);
            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }

        /// <summary>
        /// ParameterRebinder
        /// </summary>
        private class ParameterRebinder : ExpressionVisitor
        {
            /// <summary>
            /// The ParameterExpression map
            /// </summary>
            readonly Dictionary<ParameterExpression, ParameterExpression> map;
            /// <summary>
            /// Initializes a new instance of the <see cref="ParameterRebinder"/> class.
            /// </summary>
            /// <param name="map">The map.</param>
            ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
            {
                this.map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
            }
            /// <summary>
            /// Replaces the parameters.
            /// </summary>
            /// <param name="map">The map.</param>
            /// <param name="exp">The exp.</param>
            /// <returns>Expression</returns>
            public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
            {
                return new ParameterRebinder(map).Visit(exp);
            }
            /// <summary>
            /// Visits the parameter.
            /// </summary>
            /// <param name="p">The p.</param>
            /// <returns>Expression</returns>
            protected override Expression VisitParameter(ParameterExpression p)
            {
                ParameterExpression replacement;

                if (map.TryGetValue(p, out replacement))
                {
                    p = replacement;
                }
                return base.VisitParameter(p);
            }
        }
    }
}
