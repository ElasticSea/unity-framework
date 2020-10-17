using UnityEngine;

namespace Core.Util.PropertyDrawers
{
    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class CustomObjectPickerAttribute : PropertyAttribute
    {
        public enum ResultObjectType
        {
            Scene,
            Asset,
            SceneOrAsset
        };
        public System.Type[] typeRestrictions;
        public ResultObjectType resultObjectType;
 
        private CustomObjectPickerAttribute() { }
 
        public CustomObjectPickerAttribute(params System.Type[] typeRestrictions) : this(ResultObjectType.SceneOrAsset, typeRestrictions)
        {
        }
 
        public CustomObjectPickerAttribute(ResultObjectType resultObjectType, params System.Type[] typeRestrictions)
        {
            this.typeRestrictions = typeRestrictions;
            this.resultObjectType = resultObjectType;
        }
    }
}