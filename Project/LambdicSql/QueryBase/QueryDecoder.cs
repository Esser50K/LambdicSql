﻿using LambdicSql.Inside;
using System;
using System.Linq;

namespace LambdicSql.QueryBase
{
    //TODO rename internal
    public class QueryDecoder
    {
        DbInfo _db;
        ExpressionDecoder _parser;

        public string ToString(IQuery query)
        {
            return ToStringCore(query) + ";";
        }

        public virtual string CustomOperator(Type type1, string @operator, Type type2) => @operator;

        internal string ToStringCore(IQuery query)
        {
            //TODO@@ init query info.

            _db = query.Db;
            _parser = new ExpressionDecoder(_db, this);
            return string.Join(Environment.NewLine, query.GetClausesClone().Select(e=>e.ToString(_parser)).ToArray());
        }
    }
}
