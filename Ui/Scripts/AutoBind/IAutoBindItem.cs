using System.Collections.Generic;

namespace Ui.AutoBind
{
    public interface IAutoBindItem
    {
        string Name { get; }
        object Value { get; set; }
        List<object> Values { get; }
        ControlType ControlType { get; }
    }
}