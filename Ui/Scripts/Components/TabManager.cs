using System;
using System.Collections.Generic;
using System.Linq;
using Core.Extensions;
using Ui.Binding;
using UnityEngine;
using UnityEngine.UI;

namespace Ui.Components
{
    public class TabManager : MonoBehaviour, IUiBinding<IEnumerable<Tab>>
    {
        [SerializeField] private ListView switchbox;

        [SerializeField] private Tab[] tabs = new Tab[0];
        [SerializeField] private Tab[] selectedTabs= new Tab[0];
        [SerializeField] private bool dontDeactivete;
        private PropertyBinding<IEnumerable<Tab>> selectedProp;

        public IEnumerable<Tab> Tabs => tabs.ToList();
        public IEnumerable<Tab> SelectedTabs => selectedTabs.ToList();

        private void Start()
        {
            var tabsProb = new PropertyBinding<IEnumerable<Tab>>(() => tabs);
            selectedProp = new PropertyBinding<IEnumerable<Tab>>(() => SelectedTabs, go =>
            {
                selectedTabs = go.ToArray();
                SelectTab();
                OnValueChanged(Value);
            });

            tabsProb.BindProperty(switchbox.As<Tab>().List);
            selectedProp.BindProperty(switchbox.As<Tab>().Values);

            SelectTab();
        }

        private void SelectTab()
        {
            foreach (var tab in tabs)
            {
                var selected = SelectedTabs.Contains(tab);
                if (dontDeactivete)
                {
                    var group = tab.GetOrAddComponent<CanvasGroup>();
                    group.alpha = selected ? 1 : 0;
                    group.interactable = selected;
                    group.blocksRaycasts = selected;
                    
                    var layoutElement = tab.GetOrAddComponent<LayoutElement>();
                    layoutElement.ignoreLayout = selected == false;
                }
                else
                {
                    tab.gameObject.SetActive(selected);
                }
            }
        }

        public IEnumerable<Tab> Value
        {
            get => selectedProp.Value;
            set => selectedProp.Value = value;
        }

        public event Action<IEnumerable<Tab>> OnValueChanged = enumerable => { };
    }
}