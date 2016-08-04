﻿using LambdicSql.QueryBase;
using System;
using System.Linq.Expressions;

namespace LambdicSql
{
    public static class GroupByWordsExtensions
    {
        public static ISqlKeyWord<TSelected> GroupBy<TSelected>(this ISqlKeyWord<TSelected> words, params object[] target) => null;

        public static string MethodsToString(ISqlStringConverter converter, MethodCallExpression[] methods)
        {
            var method = methods[0];
            return Environment.NewLine + "GROUP BY " + converter.ToString(method.Arguments[1]);
        }
    }
}
