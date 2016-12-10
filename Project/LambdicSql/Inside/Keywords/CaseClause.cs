﻿using LambdicSql.SqlBase;
using System;
using System.Linq.Expressions;
using System.Linq;
using System.Collections.Generic;

namespace LambdicSql.Inside.Keywords
{
    static class CaseClause
    {
        internal static string ToString(ISqlStringConverter converter, MethodCallExpression[] methods)
        {
            var list = new List<string>();
            foreach (var m in methods)
            {
                var argSrc = m.Arguments.Skip(m.SkipMethodChain(0)).Select(e => converter.ToString(e)).ToArray();
                list.Add(MethodToString(m.Method.Name, argSrc));
            }
            return string.Join(string.Empty, list.ToArray());
        }

        static string MethodToString(string name, string[] argSrc)
        {
            switch (name)
            {
                case nameof(LambdicSql.Keywords.Case):
                    {
                        var text = Environment.NewLine + "\tCASE";
                        if (argSrc.Length == 1)
                        {
                            text += (" " + argSrc[0]);
                        }
                        return text;
                    }
                case nameof(LambdicSql.Keywords.When): return Environment.NewLine + "\t\tWHEN " + SqlDisplayAdjuster.AdjustSubQueryString(argSrc[0], "\t\t");
                case nameof(LambdicSql.Keywords.Then): return " THEN " + argSrc[0];
                case nameof(LambdicSql.Keywords.Else): return Environment.NewLine + "\t\tELSE " + argSrc[0];
                case nameof(LambdicSql.Keywords.End): return Environment.NewLine + "\tEND";
            }
            return null;
        }
    }
}
