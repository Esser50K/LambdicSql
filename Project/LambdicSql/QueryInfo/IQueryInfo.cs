﻿using System;

namespace LambdicSql.QueryInfo
{
    public interface IQueryInfo
    {
        DbInfo Db { get; }
        SelectInfo Select { get; }
        FromInfo From { get; }
        WhereInfo Where { get; }
        GroupByInfo GroupBy { get; }
        OrderByInfo OrderBy { get; }
    }

    public interface IQueryInfo<TSelect> : IQueryInfo
    {
        Func<IDbResult, TSelect> Create { get; }
    }
}
