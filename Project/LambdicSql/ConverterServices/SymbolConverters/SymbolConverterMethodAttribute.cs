﻿using LambdicSql.BuilderServices.Code;
using System;
using System.Linq.Expressions;

namespace LambdicSql.ConverterServices.SymbolConverters
{
    /// <summary>
    /// SQL symbol converter attribute for method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class SymbolConverterMethodAttribute : Attribute
    {
        /// <summary>
        /// Convert expression to code parts.
        /// </summary>
        /// <param name="expression">Expression.</param>
        /// <param name="converter">Expression converter.</param>
        /// <returns>Parts.</returns>
        public abstract Parts Convert(MethodCallExpression expression, ExpressionConverter converter);
    }
}
