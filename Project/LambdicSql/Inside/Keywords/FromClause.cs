﻿using LambdicSql.SqlBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LambdicSql.Inside.Keywords
{
    static class FromClause
    {
        internal static string ToString(ISqlStringConverter converter, MethodCallExpression[] methods)
        {
            var list = new List<string>();
            foreach (var m in methods)
            {
                list.Add(MethodToString(converter, m));
            }
            return string.Join(string.Empty, list.ToArray());
        }

        static string MethodToString(ISqlStringConverter converter, MethodCallExpression method)
        {
            string name = method.Method.Name;
            var startIndex = method.AdjustSqlSyntaxMethodArgumentIndex(0);
            switch (name)
            {
                case nameof(LambdicSql.Keywords.From):
                    return Environment.NewLine + "FROM " + ExpressionToTableName(converter, method.Arguments[startIndex]);
                case nameof(LambdicSql.Keywords.CrossJoin):
                    return Environment.NewLine + "\tCROSS JOIN " + ExpressionToTableName(converter, method.Arguments[startIndex]);
                case nameof(LambdicSql.Keywords.LeftJoin):
                    name = "LEFT JOIN";
                    break;
                case nameof(LambdicSql.Keywords.RightJoin):
                    name = "RIGHT JOIN";
                    break;
            }
            string[] argSrc = method.Arguments.Skip(startIndex).Select(e => converter.ToString(e)).ToArray();
            return Environment.NewLine + "\t" + name.ToUpper() + " " + ExpressionToTableName(converter, method.Arguments[startIndex]) + " ON " + argSrc[1];
        }

        //TODO refactoring.
        static string ExpressionToTableName(ISqlStringConverter decoder, Expression exp)
        {
            return AdjustSubQuery(exp, ExpressionToTableNameCore(decoder, exp));
        }

        static string ExpressionToTableNameCore(ISqlStringConverter decoder, Expression exp)
        {
            var arry = exp as NewArrayExpression;
            if (arry != null)
            {
                return string.Join(",", arry.Expressions.Select(e => ExpressionToTableName(decoder, e)).ToArray());
            }

            var text = decoder.ToString(exp);

            var methodCall = exp as MethodCallExpression;
            if (methodCall != null)
            {
                var member = methodCall.Arguments[0] as MemberExpression;
                if (member != null)
                {
                    var x = member.Member.Name;
                    return text + " " + x;
                }
                return text;
            }

            //From clause only
            var body = GetSqlExpressionBody(exp);
            if (body != null)
            {
                return text + " " + body;
            }
            return text;
        }


        //TODO refactoring.
        static string AdjustSubQuery(Expression e, string v)
        {
            if (typeof(IClauseChain).IsAssignableFrom(e.Type))
            {
                return SqlStringConverter.AdjustSubQueryString(v);
            }
            return v;
        }

        static string GetSqlExpressionBody(Expression exp)
        {
            var member = exp as MemberExpression;
            while (member != null)
            {
                if (typeof(ISqlExpressionBase).IsAssignableFrom(member.Type))
                {
                    return member.Member.Name;
                }
                member = member.Expression as MemberExpression;
            }
            return null;
        }
    }
}
