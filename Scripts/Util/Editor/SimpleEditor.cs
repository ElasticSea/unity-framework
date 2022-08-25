using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace ElasticSea.Framework.Util.Editor
{
    public abstract class SimpleEditor : UnityEditor.Editor
    {
        private object selected;
        protected abstract void OnGui();

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            OnGui();
        }
        
        protected T? SelectBox<T>(IEnumerable<T> elements, T? selected, Action<T> callback) where T : struct, Enum
        {
            GUILayout.BeginHorizontal();
            foreach (var e in elements)
            {
                var isSelected = Equals(selected, e);
                GUI.enabled = isSelected == false;
                if (GUILayout.Button(e.ToString()))
                {
                    selected = e;
                    callback(e);
                }
                GUI.enabled = true;
            }
            GUILayout.EndHorizontal();
            return selected;
        }
        
        protected void ButtonsBox<T>(IEnumerable<T> elements, Action<T> callback)
        {
            GUILayout.BeginHorizontal();
            foreach (var e in elements)
            {
                if (GUILayout.Button(e.ToString()))
                {
                    callback(e);
                }
            }
            GUILayout.EndHorizontal();
        }
    }
}