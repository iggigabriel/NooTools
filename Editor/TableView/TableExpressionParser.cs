using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Noo.Tools.Editor
{

    public static class TableExpressionParser
    {
        public static BoolExp Compile(Type type, string query)
        {
            var exp = new BoolExp();
            var ands = query.Split(new string[] { "&", "&&", "and", "AND" }, System.StringSplitOptions.RemoveEmptyEntries);

            foreach (var and in ands)
            {
                exp.Push();

                var ors = and.Split(new string[] { "|", "||", "or", "OR" }, System.StringSplitOptions.RemoveEmptyEntries);

                foreach (var or in ors)
                {
                    var cond = CompileCondition(or, type);

                    if (cond != null)
                    {
                        exp.Push(cond);
                    }
                }
            }

            return exp;
        }

        static BoolCondition CompileCondition(string query, Type type)
        {
            query = query.Trim();

            if (string.IsNullOrEmpty(query)) return null;

            var match = Regex.Match(query, "([^<>=]*)(<=|>=|<|>|==|!=)([^<>=]*)");

            if (match.Groups.Count != 4) throw new System.Exception("Invalid expression");

            var rawObjA = match.Groups[1].ToString().Trim();
            var rawObjB = match.Groups[3].ToString().Trim();
            var rawCondition = match.Groups[2].ToString().Trim();

            return new BoolCondition(type, rawObjA, rawObjB, rawCondition);
        }

        public class BoolExp
        {
            readonly List<List<BoolCondition>> conditions = new();

            public void Push()
            {
                conditions.Add(new List<BoolCondition>());
            }

            public void Push(BoolCondition cond)
            {
                conditions[conditions.Count - 1].Add(cond);
            }

            public bool Eval(object obj)
            {
                for (int i = 0; i < conditions.Count; i++)
                {
                    var ors = conditions[i];
                    if (ors.Count == 0) continue;

                    bool orResult = false;

                    for (int j = 0; j < ors.Count; j++)
                    {
                        var c = ors[j];
                        if (c.Eval(obj)) orResult = true;
                    }

                    if (orResult == false) return false;
                }

                return true;
            }
        }

        public enum Condition
        {
            Equals,
            NotEquals,
            LessThen,
            GreaterThen,
            LessThenOrEquals,
            GreaterThenOrEquals,
        }

        public class BoolCondition
        {
            public PropertyInfo a;
            public PropertyInfo b;

            public object aVal;
            public object bVal;

            public Condition c;

            public BoolCondition(Type type, string propA, string propB, string condition)
            {
                a = type.GetProperty(propA, BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public);
                b = type.GetProperty(propB, BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public);

                if (a == null && b == null) throw new Exception($"Invalid condition properties ({propA}, {propB}).");

                if (a == null)
                {
                    var bType = b.PropertyType;
                    if (bType == typeof(string))
                    {
                        aVal = propA;
                    }
                    else
                    {
                        var converter = TypeDescriptor.GetConverter(bType) ?? throw new Exception($"Invalid condition property ({propB}).");
                        aVal = converter.ConvertFromInvariantString(propA);
                    }
                }

                if (b == null)
                {
                    var aType = a.PropertyType;
                    if (aType == typeof(string))
                    {
                        bVal = propB;
                    }
                    else
                    {
                        var converter = TypeDescriptor.GetConverter(aType) ?? throw new Exception($"Invalid condition property ({propA}).");
                        bVal = converter.ConvertFromInvariantString(propB);
                    }
                }

                c = condition switch
                {
                    "==" => Condition.Equals,
                    "!=" => Condition.NotEquals,
                    "<" => Condition.LessThen,
                    ">" => Condition.GreaterThen,
                    "<=" => Condition.LessThenOrEquals,
                    ">=" => Condition.GreaterThenOrEquals,
                    _ => throw new Exception($"Invalid condition operator ({condition})."),
                };
            }

            public bool Eval(object context)
            {
                var valA = (a == null) ? aVal : a.GetValue(context);
                var valB = (b == null) ? bVal : b.GetValue(context);

                return Compare(a == null ? b.PropertyType : a.PropertyType, valA, valB);
            }

            public bool Compare(Type type, object valA, object valB)
            {
                int comparison = 0;

                if (type == typeof(string))
                {
                    comparison = ((string)valA).CompareTo(valB);
                }
                else if (type == typeof(float))
                {
                    comparison = ((float)valA).CompareTo(valB);
                }
                else if (type == typeof(int))
                {
                    comparison = ((int)valA).CompareTo(valB);
                }
                else if (type == typeof(bool))
                {
                    comparison = ((bool)valA).CompareTo(valB);
                }
                else
                {
                    if (c == Condition.Equals) return valA == valB;
                    if (c == Condition.NotEquals) return valA != valB;
                }

                return c switch
                {
                    Condition.Equals => comparison == 0,
                    Condition.NotEquals => comparison != 0,
                    Condition.GreaterThen => comparison > 0,
                    Condition.GreaterThenOrEquals => comparison >= 0,
                    Condition.LessThen => comparison < 0,
                    Condition.LessThenOrEquals => comparison <= 0,
                    _ => false,
                };
            }
        }
    }
}