using UnityEngine;

namespace Core.Ui.Components
{
    public class Tab: MonoBehaviour
    {
        [SerializeField] private string icon;
        [SerializeField] private string text;

        public string Text
        {
            get => text;
            set => text = value;
        }

        public string Icon
        {
            get => icon;
            set => icon = value;
        }
    }
}