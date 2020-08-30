using UnityEngine;

namespace _Framework.Scripts.Ui.Components.Formatters
{
    public class SelectFuncFormatter : FuncFormatter
    {
        [SerializeField] private bool uppercase = true;
        
        private SelectFuncFormatter()
        {
            Formatter = o => uppercase ? o.ToString().ToUpper() : o.ToString();
        }
    }
}