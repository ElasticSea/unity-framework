using System;
using UnityEngine;

namespace Ui.Binding
{
    public class FloatToVector2 : IUiBinding<Vector2>
    {
        private readonly IUiBinding<float> xValue;
        private readonly IUiBinding<float> yValue;

        public FloatToVector2(IUiBinding<float> xValue, IUiBinding<float> yValue)
        {
            this.xValue = xValue;
            this.yValue = yValue;

            xValue.OnValueChanged += f => OnValueChanged(Value);
            yValue.OnValueChanged += f => OnValueChanged(Value);
        }

        public Vector2 Value
        {
            get { return new Vector2(xValue.Value, yValue.Value); }
            set
            {
                xValue.Value = value.x;
                yValue.Value = value.y;
            }
        }

        public event Action<Vector2> OnValueChanged = vector2 => { };
    }
}