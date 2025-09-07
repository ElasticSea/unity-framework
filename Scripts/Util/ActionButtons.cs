using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ElasticSea.Framework.Util
{
    public class ActionButtons : MonoBehaviour
    {
        [Serializable]
        public class NamedAction
        {
            public string name;
            public Action callback;
        }

        private readonly List<NamedAction> actions = new();

        /// <summary>
        /// Register a new named action with a callback.
        /// </summary>
        public void AddAction(string name, Action callback)
        {
            actions.Add(new NamedAction { name = name, callback = callback });
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(ActionButtons))]
        private class ActionButtonsEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                DrawDefaultInspector();

                var actionButtons = (ActionButtons)target;

                if (actionButtons.actions != null)
                {
                    foreach (var action in actionButtons.actions)
                    {
                        if (!string.IsNullOrEmpty(action.name))
                        {
                            if (GUILayout.Button(action.name))
                            {
                                action.callback?.Invoke();
                            }
                        }
                    }
                }
            }
        }
#endif
    }
}