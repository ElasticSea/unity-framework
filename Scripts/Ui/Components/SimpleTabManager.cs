using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using _Framework.Scripts.Ui.Binding;

namespace _Framework.Scripts.Ui.Components
{
    public class SimpleTabManager : MonoBehaviour
    {
        [SerializeField] private ListView switchbox;

        [SerializeField] private GameObject[] tabs = new GameObject[0];
        [SerializeField] private GameObject[] selectedTabs= new GameObject[0];

        public IEnumerable<GameObject> SelectedTabs => selectedTabs.ToList();

        private void Start()
        {
            var tabsProb = new PropertyBinding<IEnumerable<GameObject>>(() => tabs);
            var selectedProp = new PropertyBinding<IEnumerable<GameObject>>(() => SelectedTabs, go =>
            {
                selectedTabs = go.ToArray();
                SelectTab();
            });

            tabsProb.BindProperty(switchbox.As<GameObject>().List);
            selectedProp.BindProperty(switchbox.As<GameObject>().Values);

            SelectTab();
        }

        private void SelectTab()
        {
            foreach (var tab in tabs)
            {
                tab.gameObject.SetActive(SelectedTabs.Contains(tab));
            }
        }
    }
}