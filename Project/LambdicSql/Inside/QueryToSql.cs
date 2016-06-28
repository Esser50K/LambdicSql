﻿using LambdicSql.QueryInfo;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LambdicSql.Inside
{
    class QueryToSql
    {
        DbInfo _db;
        
        internal string MakeQueryString(IQueryInfo query)
        {
            _db = query.Db;
            return string.Join(Environment.NewLine, new[] {
                ToString(Adjust(query.Select)),
                ToString(Adjust(query.From)),
                ToString(query.Where, "WHERE"),
                ToString(query.GroupBy),
                ToString(query.Having, "HAVING"),
                ToString(query.OrderBy)
            }.Where(e=>!string.IsNullOrEmpty(e)).ToArray());
        }

        SelectInfo Adjust(SelectInfo select)
        {
            if (select != null)
            {
                return select;
            }
            select = new SelectInfo();
            foreach (var e in _db.GetLambdaNameAndColumn())
            {
                select.Add(new SelectElementInfo(e.Key, null));
            }
            return select;
        }

        FromInfo Adjust(FromInfo from)
        {
            if (from != null)
            {
                return from;
            }

            //table count must be 1.
            if (_db.GetLambdaNameAndTable().Count != 1)
            {
                throw new NotSupportedException();
            }
            return new FromInfo(_db.GetLambdaNameAndTable().First().Value);
        }

        string ToString(SelectInfo selectInfo)
            => "SELECT " + Environment.NewLine + "\t" + string.Join("," + Environment.NewLine + "\t", selectInfo.GetElements().Select(e => ToString(e)).ToArray()) + Environment.NewLine;

        string ToString(SelectElementInfo element)
            => element.Expression == null ? element.Name : ToString(element.Expression) + " AS " + element.Name;

        string ToString(Expression exp)
            => ExpressionToSqlString.ToString(_db, exp);

        string ToString(FromInfo fromInfo)
            => string.Join(Environment.NewLine + "\t", new[] { "FROM " + fromInfo.MainTable.SqlFullName }.Concat(fromInfo.GetJoins().Select(e=>ToString(e))).ToArray()) + Environment.NewLine;

        string ToString(JoinInfo join)
            => "JOIN " + join.JoinTable.SqlFullName + " ON " + ToString(join.Condition);
        
        string ToString(ConditionClauseInfo whereInfo, string clause)
            => (whereInfo == null || whereInfo.ConditionCount == 0)?
                string.Empty:
                string.Join(Environment.NewLine + "\t", new[] { clause }.Concat(whereInfo.GetConditions().Select((e, i) => ToString(e, i))).ToArray()) + Environment.NewLine;

        string ToString(IConditionInfo condition, int index)
        {
            string text;
            var type = condition.GetType();
            if (type == typeof(ConditionInfoExpression)) text = ToString((ConditionInfoExpression)condition);
            else if (type == typeof(ConditionInfoIn)) text = ToString((ConditionInfoIn)condition);
            else if (type == typeof(ConditionInfoLike)) text = ToString((ConditionInfoLike)condition);
            else if (type == typeof(ConditionInfoBetween)) text = ToString((ConditionInfoBetween)condition);
            else throw new NotSupportedException();

            var connection = string.Empty;
            if (index != 0)
            {
                switch (condition.ConditionConnection)
                {
                    case ConditionConnection.And: connection = "AND "; break;
                    case ConditionConnection.Or: connection = "OR "; break;
                    default: throw new NotSupportedException();
                }
            }
            var not = condition.IsNot ? "NOT " : string.Empty;
            return connection + not + text;
        }

        string ToString(ConditionInfoBetween condition)
            => ToString(condition.Target) + " BETWEEN " + ToStringObject(condition.Min) + " AND " + ToStringObject(condition.Max);

        string ToString(ConditionInfoExpression condition)
            => ToString(condition.Expression);

        string ToString(ConditionInfoIn condition)
            => ToString(condition.Target) + " IN(" + MakeSqlArguments(condition.GetArguments()) + ")";

        string ToString(ConditionInfoLike condition)
            => ToString(condition.Target) + " LIKE " + ToStringObject(condition.SearchText);

        string ToString(GroupByInfo groupBy)
            => (groupBy == null || groupBy.GetElements().Length == 0) ? 
                string.Empty :
                "GROUP BY " + Environment.NewLine + "\t" + string.Join("," + Environment.NewLine + "\t", groupBy.GetElements().Select(e=>ToString(e)).ToArray()) + Environment.NewLine;

        string ToString(OrderByInfo orderBy)
            => (orderBy == null || orderBy.GetElements().Length == 0) ?
                string.Empty :
                "ORDER BY " + Environment.NewLine + "\t" + string.Join("," + Environment.NewLine + "\t", orderBy.GetElements().Select(e=>ToString(e)).ToArray()) + Environment.NewLine;

        string ToString(OrderByElement element)
            => ToString(element.Target) + " " + element.Order;

        string ToStringObject(object obj)
        {
            var exp = obj as Expression;
            return (exp != null) ? ExpressionToSqlString.ToString(_db, exp) : "'" + obj + "'";
        }

        string MakeSqlArguments(IEnumerable<object> src)
        {
            var result = new List<string>();
            foreach (var arg in src)
            {
                var col = arg as ColumnInfo;
                result.Add(col == null ? ToStringObject(arg) : col.SqlFullName);
            }
            return string.Join(", ", result.ToArray());
        }
    }
}
