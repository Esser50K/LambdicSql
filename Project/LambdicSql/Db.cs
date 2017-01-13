﻿using LambdicSql.ConverterServices;
using LambdicSql.ConverterServices.Inside;
using LambdicSql.Inside.CodeParts;
using System;
using System.Linq.Expressions;
using LambdicSql.BuilderServices.CodeParts;

namespace LambdicSql
{
    /// <summary>
    /// Sql creator.
    /// </summary>
    /// <typeparam name="T">DB's type.</typeparam>
    public class Db<T> where T : class
    {
        /// <summary>
        /// Create an expression that will be part of the query.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="expression">An expression expressing a part of query by C #.</param>
        /// <returns>An expression that will be part of the query.</returns>
        public static Sql<TResult> Sql<TResult>(Expression<Func<T, TResult>> expression)
        {
            var db = DBDefineAnalyzer.GetDbInfo<T>();
            return new Sql<TResult>(MakeSynatx(db, expression.Body));
        }

        /// <summary>
        /// Create a query.
        /// </summary>
        /// <typeparam name="TSelected">It is the type selected in the SELECT clause.</typeparam>
        /// <param name="expression">An expression expressing a query by C #.</param>
        /// <returns>A query.</returns>
        public static Sql<TSelected> Sql<TSelected>(Expression<Func<T, ClauseChain<TSelected>>> expression)
        {
            var db = DBDefineAnalyzer.GetDbInfo<T>();
            return new Sql<TSelected>(MakeSynatx(db, expression.Body));
        }

        /// <summary>
        /// Create a query.
        /// </summary>
        /// <param name="expression">An expression expressing a query by C #.</param>
        /// <returns>A query.</returns>
        public static Sql Sql(Expression<Func<T, ClauseChain<Non>>> expression)
        {
            var db = DBDefineAnalyzer.GetDbInfo<T>();
            return new Sql(MakeSynatx(db, expression.Body));
        }

        /// <summary>
        /// Create a query.
        /// Add name.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="expression">An expression expressing a query by C #.</param>
        /// <returns>A query.</returns>
        public static Sql<TResult> Sql<TResult>(Expression<Func<T, Sql<TResult>>> expression)
        {
            var db = DBDefineAnalyzer.GetDbInfo<T>();
            var core = expression.Body as MemberExpression;
            return new Sql<TResult>(core.Member.Name);
        }

        /// <summary>
        /// Create a query.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="expression">An expression expressing a query by C #.</param>
        /// <returns>A query.</returns>
        public static SqlRecursiveArguments<TResult> Sql<TResult>(Expression<Func<T, Symbol.RecursiveArguments<TResult>>> expression)
        {
            var db = DBDefineAnalyzer.GetDbInfo<T>();
            return new SqlRecursiveArguments<TResult>(MakeSynatx(db, expression.Body));
        }

        static Code MakeSynatx(DbInfo dbInfo, Expression core)
        {
            var converter = new ExpressionConverter(dbInfo);
            return core == null ? string.Empty : converter.Convert(core);
        }
    }
}
