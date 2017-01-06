﻿using LambdicSql.BuilderServices.Code;
using System.Linq;
using System.Linq.Expressions;

namespace LambdicSql.ConverterServices.SymbolConverters.Inside
{
    class PartitionByConverterAttribute : SymbolConverterMethodAttribute
    {
        public override Parts Convert(MethodCallExpression expression, ExpressionConverter converter)
        {
            var partitionBy = new VParts();
            partitionBy.Add("PARTITION BY");

            var array = expression.Arguments[0] as NewArrayExpression;
            var args = new VParts(array.Expressions.Select(e => converter.Convert(e))) { Indent = 1, Separator = "," };
            partitionBy.Add(args);

            return partitionBy;
        }
    }
}
