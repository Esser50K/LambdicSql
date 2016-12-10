﻿using LambdicSql.SqlBase;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Reflection;

namespace LambdicSql.Inside
{
    static class SqlSyntaxHelper
    {
        const BindingFlags MethodFindFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        static Dictionary<Type, Func<ISqlStringConverter, MethodCallExpression[], string>> _methodToStrings = new Dictionary<Type, Func<ISqlStringConverter, MethodCallExpression[], string>>();
        static Dictionary<Type, Func<ISqlStringConverter, MemberExpression, string>> _memberToStrings = new Dictionary<Type, Func<ISqlStringConverter, MemberExpression, string>>();
        static Dictionary<Type, Func<ISqlStringConverter, NewExpression, string>> _newToStrings = new Dictionary<Type, Func<ISqlStringConverter, NewExpression, string>>();
        static Dictionary<Type, bool> _isSqlSyntax = new Dictionary<Type, bool>();

        //TODO 本当はモジュールと組み合わせる必要があるらしい
        static Dictionary<int, bool> _canResolveSqlSyntaxMethodChain = new Dictionary<int, bool>();
        static Dictionary<int, string> _methodGroup = new Dictionary<int, string>();

        internal static bool IsSqlSyntax(this Type type)
        {
            lock (_isSqlSyntax)
            {
                bool check;
                if (!_isSqlSyntax.TryGetValue(type, out check))
                {
                    check = type.GetCustomAttributes(true).Any(e=>e is SqlSyntaxAttribute);
                    _isSqlSyntax[type] = check;
                }
                return check;
            }
        }

        internal static bool CanResolveSqlSyntaxMethodChain(this MethodInfo type)
        {
            lock (_canResolveSqlSyntaxMethodChain)
            {
                bool check;
                if (!_canResolveSqlSyntaxMethodChain.TryGetValue(type.MetadataToken, out check))
                {
                    check = type.GetCustomAttributes(true).Any(e => e is ResolveSqlSyntaxMethodChainAttribute);
                    _canResolveSqlSyntaxMethodChain[type.MetadataToken] = check;
                }
                return check;
            }
        }
        
        internal static Func<ISqlStringConverter, MethodCallExpression[], string> GetConverotrMethod(this MethodCallExpression exp)
        {
            var type = exp.Method.DeclaringType;
            lock (_methodToStrings)
            {
                Func<ISqlStringConverter, MethodCallExpression[], string> func;
                if (_methodToStrings.TryGetValue(type, out func)) return func;
                
                var methodToString = type.GetMethod("ToString", MethodFindFlags,
                    null,
                    new Type[] { typeof(ISqlStringConverter), typeof(MethodCallExpression[]) },
                    new ParameterModifier[0]);

                var arguments = new[] {
                    Expression.Parameter(typeof(ISqlStringConverter), "cnv"),
                    Expression.Parameter(typeof(MethodCallExpression[]), "exps")
                };

                func = Expression.Lambda<Func<ISqlStringConverter, MethodCallExpression[], string>>
                    (Expression.Call(null, methodToString, arguments), arguments).Compile();

                _methodToStrings.Add(type, func);

                return func;
            }
        }
        
        internal static Func<ISqlStringConverter, MemberExpression, string> GetConverotrMethod(this MemberExpression exp)
        {
            var type = exp.Member.DeclaringType;
            lock (_memberToStrings) 
            {
                Func<ISqlStringConverter, MemberExpression, string> func;
                if (_memberToStrings.TryGetValue(type, out func)) return func;

                var methodToString = type.GetMethod("ToString", MethodFindFlags,
                    null,
                    new Type[] { typeof(ISqlStringConverter), typeof(MemberExpression) },
                    new ParameterModifier[0]);

                var arguments = new[] {
                    Expression.Parameter(typeof(ISqlStringConverter), "cnv"),
                    Expression.Parameter(typeof(MemberExpression), "exps")
                };

                func = Expression.Lambda<Func<ISqlStringConverter, MemberExpression, string>>
                    (Expression.Call(null, methodToString, arguments), arguments).Compile();

                _memberToStrings.Add(type, func);

                return func;
            }
        }

        internal static Func<ISqlStringConverter, NewExpression, string> GetConverotrMethod(this NewExpression exp)
        {
            var type = exp.Constructor.DeclaringType;
            lock (_newToStrings)
            {
                Func<ISqlStringConverter, NewExpression, string> func;
                if (_newToStrings.TryGetValue(type, out func)) return func;

                var newToString = type.GetMethod("ToString", MethodFindFlags,
                    null,
                    new Type[] { typeof(ISqlStringConverter), typeof(NewExpression) },
                    new ParameterModifier[0]);

                var arguments = new[] {
                    Expression.Parameter(typeof(ISqlStringConverter), "cnv"),
                    Expression.Parameter(typeof(NewExpression), "exps")
                };

                func = Expression.Lambda<Func<ISqlStringConverter, NewExpression, string>>
                    (Expression.Call(null, newToString, arguments), arguments).Compile();
                _newToStrings.Add(type, func);

                return func;
            }
        }

        internal static string GetMethodGroupName(this MethodInfo method)
        {
            lock (_methodGroup)
            {
                string groupName;
                if (_methodGroup.TryGetValue(method.MetadataToken, out groupName)) return groupName;

                var attr = method.GetCustomAttributes(typeof(MethodGroupAttribute), true).Cast<MethodGroupAttribute>().FirstOrDefault();
                groupName = attr == null ? string.Empty : attr.GroupName;
                _methodGroup[method.MetadataToken] = groupName;
                return groupName;
            }
        }

        internal static int SkipMethodChain(this MethodCallExpression exp, int index)
        {
            var ps = exp.Method.GetParameters();
            if (0 < ps.Length && typeof(IMethodChain).IsAssignableFrom(ps[0].ParameterType)) return index + 1;
            else return index;
        }
    }
}
