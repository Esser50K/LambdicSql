﻿using LambdicSql.QueryBase;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Linq;

namespace LambdicSql.Inside
{
    static class RefrectionHelpercs
    {
        static Dictionary<Type, Func<ISqlStringConverter, MethodCallExpression[], string>> _methodToStrings =
                new Dictionary<Type, Func<ISqlStringConverter, MethodCallExpression[], string>>();

        static Dictionary<Type, Func<ISqlStringConverter, NewExpression, string>> _newToStrings =
                new Dictionary<Type, Func<ISqlStringConverter, NewExpression, string>>();

        static Dictionary<Type, bool> _isSqlSyntax = new Dictionary<Type, bool>();

        internal static string GetPropertyName(this MethodInfo method)
            => (method.Name.IndexOf("get_") == 0) ?
                method.Name.Substring(4) : method.Name;

        internal static bool IsSqlSyntaxResolver(this MethodInfo method)
        {
            lock (_isSqlSyntax)
            {
                bool check;
                if (!_isSqlSyntax.TryGetValue(method.DeclaringType, out check))
                {
                    check = method.DeclaringType.GetCustomAttributes(true).Any(e=>e is SqlSyntaxAttribute);
                    _isSqlSyntax[method.DeclaringType] = check;
                }
                if (check) return true;
            }

            var ps = method.GetParameters();
            return method.IsStatic &&
                0 < ps.Length &&
                typeof(ISqlSyntax).IsAssignableFrom(ps[0].ParameterType);
        }

        internal static Func<ISqlStringConverter, MethodCallExpression[], string>
            GetMethodsToString(this MethodCallExpression exp)
        {
            var type = exp.Method.DeclaringType;
            lock (_methodToStrings)
            {
                Func<ISqlStringConverter, MethodCallExpression[], string> func;
                if (_methodToStrings.TryGetValue(type, out func)) return func;

                var methodToString = type.GetMethod("MethodsToString");
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

        internal static Func<ISqlStringConverter, NewExpression, string>
            GetNewToString(this NewExpression exp)
        {
            var type = exp.Constructor.DeclaringType;
            lock (_newToStrings)
            {
                Func<ISqlStringConverter, NewExpression, string> func;
                if (_newToStrings.TryGetValue(type, out func)) return func;

                var newToString = type.GetMethod("NewToString");
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
    }
}
