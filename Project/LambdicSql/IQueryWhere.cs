﻿namespace LambdicSql
{
    public interface IQueryWhere<TDB, TSelect> : IQueryWhereEnd<TDB, TSelect>
        where TDB : class
        where TSelect : class
    { }
}
