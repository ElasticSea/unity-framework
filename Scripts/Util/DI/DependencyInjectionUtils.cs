using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ElasticSea.Framework.Scripts.Util.DI
{
    public static class DependencyInjectionUtils
    {
        private static Dictionary<Type, List<(Type fieldType, Action<object, object> action)>> dict = new();
        private static List<(Type fieldType, Action<object, object> action)> GetSetters(object mb)
        {
            var fields = mb.GetType()
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(f => f.GetCustomAttribute<DependencyInjectionAttribute>() != null)
                .Select(f => (f.FieldType, (Action<object, object>) ((m, value) => f.SetValue(m, value))));
                
            var properties = mb.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(f => f.GetCustomAttribute<DependencyInjectionAttribute>() != null)
                .Select(f => (f.PropertyType, (Action<object, object>) ((m, value) => f.SetValue(m, value))));

            return fields.Concat(properties).ToList();
        } 
        
        public static void InjectGameobject(GameObject target, Dictionary<Type, object> map)
        {
            var monoBehaviours = target
                .GetComponentsInChildren<MonoBehaviour>(true)
                .Where(mb => mb != null);
            
            foreach (var mb in monoBehaviours)
            {
                Inject(mb, map);
            }
        }
        
        public static void Inject(object target, Dictionary<Type, object> map)
        {
            if (dict.ContainsKey(target.GetType()) == false)
            {
                dict[target.GetType()] = GetSetters(target);
            }

            var list = dict[target.GetType()];
            for (var i = 0; i < list.Count; i++)
            {
                var (fieldType, action) = list[i];
                if (map.ContainsKey(fieldType))
                {
                    action(target, map[fieldType]);
                }
            }
        }
    }
}