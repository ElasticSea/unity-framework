using System.Collections.Generic;

namespace Core.Ui.AutoBind
{
    public interface IAutoBindItem
    {
        string Name { get; }
        object Value { get; set; }
        List<object> Values { get; }
        ControlType ControlType { get; }
    }
}