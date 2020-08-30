using UnityEngine;

namespace _Framework.Scripts.Ui.Components
{
    public class GameObjectToggle : MonoBehaviour
    {
        [SerializeField] private bool inverted; 
        [SerializeField] private GameObject targetObject;
        [SerializeField] private ToggleBase toggle;
        
        private void Awake()
        {
            toggle.OnClick += () =>
            {
                targetObject.SetActive(inverted == toggle.Selected);
            };
        }
    }
}