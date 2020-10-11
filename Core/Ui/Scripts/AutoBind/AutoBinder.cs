using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Core.Extensions;

namespace Core.Ui.AutoBind
{
    public class AutoBinder
    {
        public static List<IAutoBindItem> Build(object target)
        {
            var attributes = target.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
                .Select(prop =>
                {
                    var attribute = prop.GetCustomAttribute<AutoBindAttribute>();
                    return (attribute, prop);
                })
                .Where(pair => pair.attribute != null && pair.attribute.Skip == false);

            return attributes
                .Select(a => CreateItem(target, a.prop, a.attribute))
                .ToList();
        }

        private static IAutoBindItem CreateItem(object target, PropertyInfo prop, AutoBindAttribute configAttribute)
        {
            var type = prop.PropertyType;

            var getter = Delegate.CreateDelegate(
                typeof(Func<>).MakeGenericType(type), target,
                prop.GetGetMethod(true));
            var setter = Delegate.CreateDelegate(
                typeof(Action<>).MakeGenericType(type), target,
                prop.GetSetMethod(true));

            var options = configAttribute.Values.ToList();
            var optionsType = configAttribute.Control;

            var converted = ConvertList(options, type);

            var configDefinition = typeof(AutoBindItem<>).MakeGenericType(type);
            var configItem =
                Activator.CreateInstance(configDefinition, prop.Name.SplitCamelCase().ToUpper(), type, setter, getter, converted, optionsType) as
                    IAutoBindItem;

            return configItem;
        }
        
        private static object ConvertList(List<object> value, Type type)
        {
            var listType = typeof(List<>);
            var constructedListType = listType.MakeGenericType(type);
            
            var list = (IList)Activator.CreateInstance(constructedListType);
            if (value == null) return list;
            foreach (var item in value)
            {
                list.Add(item);
            }
            return list;
        }
    }
}