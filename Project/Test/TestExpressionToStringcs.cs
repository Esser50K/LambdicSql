﻿using LambdicSql;
using LambdicSql.Inside;
using LambdicSql.QueryInfo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq.Expressions;

namespace Test
{
    [TestClass]
    public class TestExpressionToStringcs
    {
        [TestMethod]
        public void TestDB()
        {
            Sql.Using(() => new
            {
                table1 = new
                {
                    col1 = default(string)
                }
            }).ToSqlString(db => db.table1).Is("table1");
        }

        [TestMethod]
        public void TestColumn()
        {
            Sql.Using(() => new
            {
                table1 = new
                {
                    col1 = default(string)
                }
            }).ToSqlString(db => db.table1.col1).Is("table1.col1");
        }

        [TestMethod]
        public void TestConstant()
        {
            var query = Sql.Using(() => new
            {
                table1 = new
                {
                    col1 = default(string)
                }
            });
            query.ToSqlString(db => 1).Is("'1'");
            query.ToSqlString(db => "xxx").Is("'xxx'");
        }

        [TestMethod]
        public void TestNodeType()
        {
            var query = Sql.Using(() => new
            {
                table1 = new
                {
                    col1 = default(int),
                    col2 = default(bool)
                }
            });
            query.ToSqlString(db => db.table1.col1 == 1).Is("(table1.col1) = ('1')");
            query.ToSqlString(db => db.table1.col1 != 1).Is("(table1.col1) != ('1')");
            query.ToSqlString(db => db.table1.col1 < 1).Is("(table1.col1) < ('1')");
            query.ToSqlString(db => db.table1.col1 <= 1).Is("(table1.col1) <= ('1')");
            query.ToSqlString(db => db.table1.col1 > 1).Is("(table1.col1) > ('1')");
            query.ToSqlString(db => db.table1.col1 >= 1).Is("(table1.col1) >= ('1')");
            query.ToSqlString(db => db.table1.col1 + 1).Is("(table1.col1) + ('1')");
            query.ToSqlString(db => db.table1.col1 - 1).Is("(table1.col1) - ('1')");
            query.ToSqlString(db => db.table1.col1 * 1).Is("(table1.col1) * ('1')");
            query.ToSqlString(db => db.table1.col1 / 1).Is("(table1.col1) / ('1')");
            query.ToSqlString(db => db.table1.col1 % 1).Is("(table1.col1) % ('1')");
            query.ToSqlString(db => db.table1.col2 && false).Is("(table1.col2) AND ('False')");
            query.ToSqlString(db => db.table1.col2 || false).Is("(table1.col2) OR ('False')");
        }

        [TestMethod]
        public void TestBinary()
        {
            var query = Sql.Using(() => new
            {
                table1 = new
                {
                    col1 = default(int)
                }
            });
            query.ToSqlString(db => db.table1.col1 == 2 && (db.table1.col1 == 3 || db.table1.col1 == 4)).
                Is("((table1.col1) = ('2')) AND (((table1.col1) = ('3')) OR ((table1.col1) = ('4')))");
        }

        [TestMethod]
        public void TestMethod()
        {
            var query = Sql.Using(() => new
            {
                table1 = new
                {
                    col1 = default(int)
                }
            });
            query.ToSqlString((db, func) => func.Sum(1)).Is("Sum('1')");
        }
    }

    interface IFuncs { }

    static class FuncsExtensions
    { 
        public static T Sum<T>(this IFuncs f, T t) { return default(T); }
    }

    static class ExpressionTestExtensions
    {
        internal static string ToSqlString<T, TRet>(this IQueryStart<T, T> query, Expression<Func<T, TRet>> exp)
            where T : class
        {
            var info = query as IQueryInfo;
            return TestAdaptor.ToSqlString(info.Db, exp.Body);
        }

        internal static string ToSqlString<T, TRet>(this IQueryStart<T, T> query, Expression<Func<T, IFuncs, TRet>> exp)
            where T : class
        {
            var info = query as IQueryInfo;
            return TestAdaptor.ToSqlString(info.Db, exp.Body);
        }
    }
}
