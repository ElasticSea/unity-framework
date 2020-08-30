using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using _Framework.Scripts.Extensions;
using _Framework.Scripts.Ui.Binding;

namespace _Framework.Scripts.Ui.Components
{
    public class 
        
        Slider : MonoBehaviour, IUiBinding<float>, IClickable, IPointerClickHandler, IPointerUpHandler
    {
        [SerializeField] private UnityEngine.UI.Slider slider;

        public float Min = 0;
        public float Max = 1;
        [SerializeField, Obsolete] private string Format = "";
        public string FullFormat = "{0:P0}";

        [FormerlySerializedAs("BackgroundText")] [SerializeField] private Text backgroundText;
        [FormerlySerializedAs("ForegroundText")] [SerializeField] private  Text foregroundText;
        [FormerlySerializedAs("ForegroundFill")] [SerializeField] private  Image foregroundFill;
        private bool ignoreCallback;

        [SerializeField] private bool updateOnMouseUp;

        public float Value
        {
            get => PercToValue(slider.value);
            set
            {
                ignoreCallback = true;
                UpdateIt(false, value);
                ignoreCallback = false;
            }
        }

        private float PercToValue(float value) => value * (Max - Min) + Min;
        private float ValueToPerc(float value) => (value - Min) / (Max - Min);
        public event Action<float> OnValueChanged = f => { };

        private void Awake()
        {
            slider.onValueChanged.AddListener(value =>
            {
                if (ignoreCallback) return;
                
                UpdateIt(true, PercToValue(slider.value));
            });
        }

        private void OnRectTransformDimensionsChange()
        {
            var newWidth = GetComponent<RectTransform>().GetWidth();
            foregroundText.rectTransform.SetWidth(newWidth);
            foregroundFill.rectTransform.SetWidth(newWidth);
        }

        public void OnPointerClick(PointerEventData eventData) => OnClick();

        public event Action OnClick = () => { };

        private void UpdateIt(bool internalCall, float value)
        {
            var format = Format.IsNullOrEmpty() == false ? "{0:" + Format + "}" : FullFormat;
            var text = string.Format(format, value);
            backgroundText.text = text;
            foregroundText.text = text;

            if(internalCall == false)
                slider.value = ValueToPerc(value);
            
            if (ignoreCallback == false && internalCall)
            {
                if (updateOnMouseUp == false)
                {
                    OnValueChanged(Value);
                }
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (updateOnMouseUp)
            {
                OnValueChanged(Value);
            }
        }
    }
}