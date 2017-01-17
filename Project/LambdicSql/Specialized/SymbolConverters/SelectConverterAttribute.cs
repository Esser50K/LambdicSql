﻿using LambdicSql.BuilderServices.CodeParts;
using LambdicSql.ConverterServices;
using LambdicSql.ConverterServices.SymbolConverters;
using LambdicSql.ConverterServices.Inside;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LambdicSql.ConverterServices.Inside.CodeParts;
using static LambdicSql.BuilderServices.Inside.PartsFactoryUtils;

namespace LambdicSql.Specialized.SymbolConverters
{
    /// <summary>
    /// Converter for SELECT clause conversion.
    /// </summary>
    public class SelectConverterAttribute : MethodConverterAttribute
    {
        /// <summary>
        /// Convert expression to code.
        /// </summary>
        /// <param name="expression">Expression.</param>
        /// <param name="converter">Expression converter.</param>
        /// <returns>Parts.</returns>
        public override ICode Convert(MethodCallExpression expression, ExpressionConverter converter)
        {
            //ALL, DISTINCT, TOP
            var modify = new List<Expression>();
            for (int i = expression.SkipMethodChain(0); i < expression.Arguments.Count - 1; i++)
            {
                modify.Add(expression.Arguments[i]);
            }

            var select = LineSpace(new [] { "SELECT".ToCode() }.Concat(modify.Select(e => converter.ConvertToCode(e))).ToArray());

            //select elemnts.
            var selectTargets = expression.Arguments[expression.Arguments.Count - 1];

            //*
            if (typeof(IAsterisk).IsAssignableFrom(selectTargets.Type))
            {
                select.Add("*".ToCode());
                return new SelectClauseCode(select);
            }
            //new []{ a, b, c} recursive.
            else if (selectTargets.Type.IsArray)
            {
                var newArrayExp = selectTargets as NewArrayExpression;
                var elements = new VCode(newArrayExp.Expressions.Select(x => converter.ConvertToCode(x)).ToArray()) { Indent = 1, Separator = "," };
                return new SelectClauseCode(new VCode(select, elements));
            }
            //new { item = db.tbl.column }
            else
            {
                var createInfo = ObjectCreateAnalyzer.MakeSelectInfo(selectTargets);
                var elements = new VCode(createInfo.Members.Select(e => ConvertSelectedElement(converter, e))) { Indent = 1, Separator = "," };
                return new SelectClauseCode(new VCode(select, elements));
            }
        }

        static ICode ConvertSelectedElement(ExpressionConverter converter, ObjectCreateMemberInfo element)
        {
            //single select.
            //for example, COUNT(*).
            if (string.IsNullOrEmpty(element.Name)) return converter.ConvertToCode(element.Expression);

            //normal select.
            return LineSpace(converter.ConvertToCode(element.Expression), "AS".ToCode(), element.Name.ToCode());
        }
    }
}
