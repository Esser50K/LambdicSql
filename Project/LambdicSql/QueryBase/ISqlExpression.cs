﻿namespace LambdicSql.QueryBase
{
    public interface ISqlExpression
    {
        IQuery Query { get; }
        string ToString(ISqlStringConverter decoder);
    }

    public interface ISqlExpression<TDB> : ISqlExpression { }
    public interface ISqlExpression<TDB, TResult> : ISqlExpression { }
}
