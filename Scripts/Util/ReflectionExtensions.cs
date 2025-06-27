using System;
using System.Reflection;

namespace ElasticSea.Framework.Util
{
    public static class ReflectionExtensions
    {
        public static void CallPrivateMethod(this object instance, string name)
        {
            instance.CallPrivateMethod(name, null);
        }
        
        public static void CallPrivateMethod(this object instance, string name, params object[] parameters)
        {
            instance.GetType().GetMethod(name, BindingFlags.NonPublic | BindingFlags.Instance).Invoke(instance, parameters);
        }
        
        public static void SetPrivateField(this object instance, string name, object value)
        {
            instance.GetType().GetField(name, BindingFlags.NonPublic | BindingFlags.Instance).SetValue(instance, value);
        }
        
        public static T GetPrivateField<T>(this object instance, string name)
        {
            return (T) instance.GetType().GetField(name, BindingFlags.NonPublic | BindingFlags.Instance).GetValue(instance);
        }
        
        public static void SetStaticPrivateField(this Type type, string name, object value)
        {
            type.GetField(name, BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, value);
        }
    }
}