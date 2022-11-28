using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ElasticSea.Framework.Scripts.Util
{
    public class DebugActions : MonoBehaviour
    {
        [SerializeField] private Transform container;
        [SerializeField] private Button buttonPrefab;
        
        public void AddAction(string action, Action callback)
        {
            var button = Instantiate(buttonPrefab);
            button.onClick.AddListener(() => callback());
            button.GetComponentInChildren<TMP_Text>().text = action;
            button.transform.SetParent(container);
        }
    }
}