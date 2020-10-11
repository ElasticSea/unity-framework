using UnityEngine;

namespace Core.Ui.AutoBind
{
    public class AutoBindUi : MonoBehaviour
    {
        [SerializeField] private AutoBindUiItem item;

        public void Bind(object target)
        {
            var binds = AutoBinder.Build(target);
            foreach (var bind in binds)
            {
                var instance = Instantiate(item, transform);
                instance.SetItem(bind);
            }
        }
    }
}