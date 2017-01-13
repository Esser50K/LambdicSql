﻿using LambdicSql.BuilderServices;
using LambdicSql.BuilderServices.BasicCode;

namespace LambdicSql.ConverterServices.Inside.Code
{
    class SelectClauseCode : BuilderServices.BasicCode.Code
    {
        BuilderServices.BasicCode.Code _core;

        internal SelectClauseCode(BuilderServices.BasicCode.Code core)
        {
            _core = core;
        }

        public override bool IsEmpty => _core.IsEmpty;

        public override bool IsSingleLine(BuildingContext context) => _core.IsSingleLine(context);

        public override string ToString(bool isTopLevel, int indent, BuildingContext context) => _core.ToString(isTopLevel, indent, context);

        public override Code ConcatAround(string front, string back) => new SelectClauseCode(_core.ConcatAround(front, back));

        public override Code ConcatToFront(string front) => new SelectClauseCode(_core.ConcatToFront(front));

        public override Code ConcatToBack(string back) => new SelectClauseCode(_core.ConcatToBack(back));

        public override Code Customize(ICodeCustomizer customizer) => new SelectClauseCode(_core.Customize(customizer));
    }
}
