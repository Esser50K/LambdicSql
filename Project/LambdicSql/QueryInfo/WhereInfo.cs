﻿using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LambdicSql.QueryInfo
{
    public class WhereInfo
    {
        bool _isNot;
        ConditionConnection _nextConnection;

        bool IsNot
        {
            get
            {
                var isNot = _isNot;
                _isNot = false;
                return isNot;
            }
        }

        public List<IConditionInfo> Conditions { get; } = new List<IConditionInfo>();

        public WhereInfo() { }

        public WhereInfo(BinaryExpression exp)
        {
            Conditions.Add(new ConditionInfoBinary(false, ConditionConnection.And, exp));
        }

        public WhereInfo Clone()
        {
            var clone = new WhereInfo();
            clone.Conditions.AddRange(Conditions);
            return clone;
        }

        internal void And(BinaryExpression exp)
            => Conditions.Add(new ConditionInfoBinary(IsNot, ConditionConnection.And, exp));

        internal void Or(BinaryExpression exp)
            =>Conditions.Add(new ConditionInfoBinary(IsNot, ConditionConnection.Or, exp));

        internal void And()
            => _nextConnection = ConditionConnection.And;

        internal void Or()
            => _nextConnection = ConditionConnection.Or;

        internal void Not()
            => _isNot = true;

        internal void In<TLeft>(string target, TLeft[] inArguments)
            => Conditions.Add(new ConditionInfoIn(IsNot, _nextConnection, target, inArguments.Cast<object>().ToList()));

        internal void Like(string target, string serachText)
            => Conditions.Add(new ConditionInfoLike(IsNot, _nextConnection, target, serachText));
    }
}
