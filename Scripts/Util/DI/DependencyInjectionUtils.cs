using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ElasticSea.Framework.Scripts.Util.DI
{
    public static class DependencyInjectionUtils
    {
        public static void Inject(GameObject target, Dictionary<Type, object> map)
        {
            var monoBehaviours = target
                .GetComponentsInChildren<MonoBehaviour>(true)
                .Where(mb => mb != null);
            
            foreach (var mb in monoBehaviours)
            {
                var fields = mb.GetType()
                    .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(f => f.GetCustomAttribute<DependencyInjectionAttribute>() != null);
                
                foreach (var f in fields)
                {
                    if (map.ContainsKey(f.FieldType))
                    {
                        f.SetValue(mb, map[f.FieldType]);
                    }
                }
                
                var properties = mb.GetType()
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(f => f.GetCustomAttribute<DependencyInjectionAttribute>() != null);
                
                foreach (var p in properties)
                {
                    if (map.ContainsKey(p.PropertyType))
                    {
                        p.SetValue(mb, map[p.PropertyType]);
                    }
                }
            }
        }
    }
}