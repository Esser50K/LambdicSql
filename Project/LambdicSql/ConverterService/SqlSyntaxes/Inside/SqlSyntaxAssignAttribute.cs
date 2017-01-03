﻿using LambdicSql.SqlBuilder.Parts;
using System.Linq.Expressions;

namespace LambdicSql.ConverterService.SqlSyntaxes.Inside
{

    class SqlSyntaxAssignAttribute : SqlSyntaxConverterNewAttribute
    {
        public override BuildingParts Convert(ExpressionConverter converter, NewExpression exp)
        {
            BuildingParts arg1 = converter.Convert(exp.Arguments[0]).Customize(new CustomizeColumnOnly());
            return new HBuildingParts(arg1, "=", converter.Convert(exp.Arguments[1])) { Separator = " " };
        }
    }
}
