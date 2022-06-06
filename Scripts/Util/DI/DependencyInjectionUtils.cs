using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ElasticSea.Framework.Scripts.Util.DI
{
    public static class DependencyInjectionUtils
    {
        private static Dictionary<Type, List<(Type fieldType, Action<MonoBehaviour, object> action)>> dict = new Dictionary<Type, List<(Type fieldType, Action<MonoBehaviour, object> action)>>();
        private static List<(Type fieldType, Action<MonoBehaviour, object> action)> GetSetters(MonoBehaviour mb)
        {
            var fields = mb.GetType()
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(f => f.GetCustomAttribute<DependencyInjectionAttribute>() != null)
                .Select(f => (f.FieldType, (Action<MonoBehaviour, object>) ((m, value) => f.SetValue(m, value))));
                
            var properties = mb.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(f => f.GetCustomAttribute<DependencyInjectionAttribute>() != null)
                .Select(f => (f.PropertyType, (Action<MonoBehaviour, object>) ((m, value) => f.SetValue(m, value))));

            return fields.Concat(properties).ToList();
        } 
        
        public static void Inject(GameObject target, Dictionary<Type, object> map)
        {
            var monoBehaviours = target
                .GetComponentsInChildren<MonoBehaviour>(true)
                .Where(mb => mb != null);
            
            foreach (var mb in monoBehaviours)
            {
                if (dict.ContainsKey(mb.GetType()) == false)
                {
                    dict[mb.GetType()] = GetSetters(mb);
                }

                var list = dict[mb.GetType()];
                for (var i = 0; i < list.Count; i++)
                {
                    var (fieldType, action) = list[i]; 
                    if (map.ContainsKey(fieldType))
                    {
                        action(mb, map[fieldType]);
                    }
                }
            }
        }
    }
}