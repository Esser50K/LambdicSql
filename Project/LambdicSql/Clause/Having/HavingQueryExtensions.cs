﻿using LambdicSql.Clause.Having;
using LambdicSql.QueryBase;
using System;
using System.Linq.Expressions;

namespace LambdicSql
{
    public static class HavingQueryExtensions
    {
        public static IQuery<TDB, TSelect, HavingClause> Having<TDB, TSelect>(this IQuery<TDB, TSelect> query)
            where TDB : class
            where TSelect : class
            => new ClauseMakingQuery<TDB, TSelect, HavingClause>(query, new HavingClause());

        public static IQuery<TDB, TSelect, HavingClause> Having<TDB, TSelect>(this IQuery<TDB, TSelect> query, Expression<Func<TDB, bool>> condition)
            where TDB : class
            where TSelect : class
            => new ClauseMakingQuery<TDB, TSelect, HavingClause>(query, new HavingClause(condition.Body));

        public static IQuery<TDB, TSelect, HavingClause> Not<TDB, TSelect>(this IQuery<TDB, TSelect, HavingClause> query)
            where TDB : class
            where TSelect : class
             => query.CustomClone(e => e.Not());

        public static IQuery<TDB, TSelect, HavingClause> And<TDB, TSelect>(this IQuery<TDB, TSelect, HavingClause> query, Expression<Func<TDB, bool>> condition)
            where TDB : class
            where TSelect : class
             => query.CustomClone(e => e.And(condition.Body));

        public static IQuery<TDB, TSelect, HavingClause> And<TDB, TSelect>(this IQuery<TDB, TSelect, HavingClause> query, bool isEnabled, Expression<Func<TDB, bool>> condition)
            where TDB : class
            where TSelect : class
             => isEnabled ? query.CustomClone(e => e.And(condition.Body)) : query;

        public static IQuery<TDB, TSelect, HavingClause> And<TDB, TSelect>(this IQuery<TDB, TSelect, HavingClause> query)
            where TDB : class
            where TSelect : class
             => query.CustomClone(e => e.And());

        public static IQuery<TDB, TSelect, HavingClause> And<TDB, TSelect>(this IQuery<TDB, TSelect, HavingClause> query, bool isEnabled)
            where TDB : class
            where TSelect : class
             => isEnabled ? query.CustomClone(e => e.And()) : query.CustomClone(e => e.Skip());

        public static IQuery<TDB, TSelect, HavingClause> Or<TDB, TSelect>(this IQuery<TDB, TSelect, HavingClause> query, Expression<Func<TDB, bool>> condition)
            where TDB : class
            where TSelect : class
             => query.CustomClone(e => e.Or(condition.Body));

        public static IQuery<TDB, TSelect, HavingClause> Or<TDB, TSelect>(this IQuery<TDB, TSelect, HavingClause> query, bool isEnabled, Expression<Func<TDB, bool>> condition)
            where TDB : class
            where TSelect : class
             => isEnabled ? query.CustomClone(e => e.Or(condition.Body)) : query;

        public static IQuery<TDB, TSelect, HavingClause> Or<TDB, TSelect>(this IQuery<TDB, TSelect, HavingClause> query)
            where TDB : class
            where TSelect : class
             => query.CustomClone(e => e.Or());

        public static IQuery<TDB, TSelect, HavingClause> Or<TDB, TSelect>(this IQuery<TDB, TSelect, HavingClause> query, bool isEnabled)
            where TDB : class
            where TSelect : class
             => isEnabled ? query.CustomClone(e => e.Or()) : query.CustomClone(e => e.Skip());

        public static IQuery<TDB, TSelect, HavingClause> BlockStart<TDB, TSelect>(this IQuery<TDB, TSelect, HavingClause> query)
            where TDB : class
            where TSelect : class
             => query.CustomClone(e => e.BlockStart());
        
        public static IQuery<TDB, TSelect, HavingClause> BlockEnd<TDB, TSelect>(this IQuery<TDB, TSelect, HavingClause> query)
            where TDB : class
            where TSelect : class
             => query.CustomClone(e => e.BlockEnd());
    }
}
