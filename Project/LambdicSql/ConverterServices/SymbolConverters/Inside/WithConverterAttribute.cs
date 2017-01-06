﻿using LambdicSql.BuilderServices.Code;
using LambdicSql.BuilderServices.Code.Inside;
using System.Collections.Generic;
using System.Linq.Expressions;
using static LambdicSql.BuilderServices.Code.Inside.PartsFactoryUtils;

namespace LambdicSql.ConverterServices.SymbolConverters.Inside
{
    class WithConverterAttribute : SymbolConverterMethodAttribute
    {
        public override Parts Convert(MethodCallExpression expression, ExpressionConverter converter)
        {
            var arry = expression.Arguments[0] as NewArrayExpression;
            return arry == null ? ConvertRecurciveWith(expression, converter) : ConvertNormalWith(converter, arry);
        }

        static Parts ConvertNormalWith(ExpressionConverter converter, NewArrayExpression arry)
        {
            var with = new VParts() { Indent = 1, Separator = "," };
            var names = new List<string>();
            foreach (var e in arry.Expressions)
            {
                var table = converter.Convert(e);
                var body = FromConverterAttribute.GetSubQuery(e);
                names.Add(body);
                with.Add(Clause(LineSpace(body, "AS"), table));
            }
            return new WithEntriedParts(new VParts("WITH", with), names.ToArray());
        }

        static Parts ConvertRecurciveWith(MethodCallExpression expression, ExpressionConverter converter)
        {
            var table = converter.Convert(expression.Arguments[0]);
            var sub = FromConverterAttribute.GetSubQuery(expression.Arguments[0]);
            var with = new VParts() { Indent = 1 };
            with.Add(Clause(LineSpace(new RecursiveTargetParts(Line(sub, table)), "AS"), converter.Convert(expression.Arguments[1])));
            return new WithEntriedParts(new VParts("WITH", with), new[] { sub });
        }
    }
}
