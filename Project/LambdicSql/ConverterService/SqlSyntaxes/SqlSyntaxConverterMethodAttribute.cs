﻿using LambdicSql.SqlBuilder.Parts;
using System;
using System.Linq.Expressions;

namespace LambdicSql.ConverterService.SqlSyntaxes
{
    /// <summary>
    /// SQL syntax attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class SqlSyntaxConverterMethodAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="converter"></param>
        /// <param name="exp"></param>
        /// <returns></returns>
        public abstract BuildingParts Convert(ExpressionConverter converter, MethodCallExpression exp);
    }
}
