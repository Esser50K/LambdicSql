﻿using LambdicSql.BuilderServices.Parts;
using System.Linq.Expressions;
using static LambdicSql.BuilderServices.Parts.Inside.BuildingPartsFactoryUtils;

namespace LambdicSql.ConverterServices.SqlSyntaxes.Inside
{
    class SqlSyntaxValuesAttribute : SqlSyntaxConverterMethodAttribute
    {
        public override BuildingParts Convert(MethodCallExpression expression, ExpressionConverter converter)
        {
            var values = Func("VALUES", converter.Convert(expression.Arguments[1]));
            values.Indent = 1;
            return values;
        }
    }
}
