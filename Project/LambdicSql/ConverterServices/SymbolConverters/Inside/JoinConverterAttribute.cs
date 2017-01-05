﻿using LambdicSql.ConverterServices.Inside;
using LambdicSql.BuilderServices.Syntaxes;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LambdicSql.ConverterServices.SymbolConverters.Inside
{
    class JoinConverterAttribute : SymbolConverterMethodAttribute
    {
        public string Name { get; set; }

        public override Syntax Convert(MethodCallExpression expression, ExpressionConverter converter)
        {
            var startIndex = expression.SkipMethodChain(0);
            var table = FromConverterAttribute.ConvertTable(converter, expression.Arguments[startIndex]);
            var condition = ((startIndex + 1) < expression.Arguments.Count) ? converter.Convert(expression.Arguments[startIndex + 1]) : null;

            var join = new HSyntax() { IsFunctional = true, Separator = " ", Indent = 1 };
            join.Add(Name);
            join.Add(table);
            if (condition != null)
            {
                join.Add("ON");
                join.Add(condition);
            }
            return join;
        }
    }
}
