using System;
using System.Collections.Generic;

namespace ElasticSea.Framework.Extensions
{
    public static class TypeExtensions
    {
        
        public static string GetNameWithoutGenericArity(this Type t)
        {
            var name = t.Name;
            var index = name.IndexOf('`');
            return index == -1 ? name : name.Substring(0, index);
        }
        
        public static string SimpleName(this Type t)
        {
            return t.GetNameWithoutGenericArity().SplitPascalCase();
        }
        
        private static readonly Dictionary<Type, string> TypeAliases = new Dictionary<Type, string>
        {
            { typeof(byte), "byte" },
            { typeof(sbyte), "sbyte" },
            { typeof(short), "short" },
            { typeof(ushort), "ushort" },
            { typeof(int), "int" },
            { typeof(uint), "uint" },
            { typeof(long), "long" },
            { typeof(ulong), "ulong" },
            { typeof(float), "float" },
            { typeof(double), "double" },
            { typeof(decimal), "decimal" },
            { typeof(object), "object" },
            { typeof(bool), "bool" },
            { typeof(char), "char" },
            { typeof(string), "string" },
            { typeof(void), "void" }
        };
        
        public static string GetSimpleAliasName(this Type type)
        {
            return (TypeAliases.ContainsKey(type) ? TypeAliases[type] : type.GetNameWithoutGenericArity()).SplitPascalCase();
        }
    }
}