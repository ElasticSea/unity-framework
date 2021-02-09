using System;
using UnityEngine;

namespace ElasticSea.Framework.Extensions
{
    public static class ComponentExtensions
    {
        public static Component GetOrAddComponent(this Component c, Type componentType)
        {
            return c.gameObject.GetOrAddComponent(componentType);
        }

        public static T GetOrAddComponent<T>(this Component c) where T : Component
        {
            return c.gameObject.GetOrAddComponent<T>();
        }	
    }
}