using System;

namespace Core.Ui.AutoBind
{
    public class AutoBindAttribute : Attribute
    {
        public object[] Values { get; set; } = new object[0];

        public bool Skip { get; set; }
        
        public ControlType Control { get; set; }

        public float Min { get; set; }
        
        public float Max { get; set; }
    }
}