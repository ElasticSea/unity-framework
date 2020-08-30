using System;
using UnityEngine;
using UnityEngine.UI;
using _Framework.Scripts.Ui.Binding;

namespace _Framework.Scripts.Ui.Components
{
    public class TextureImage : MonoBehaviour, IUiBinding<Texture>
    {
        [SerializeField] private RawImage image;

        public Texture Value
        {
            get => image.texture;
            set => image.texture = value;
        }

        public event Action<Texture> OnValueChanged = value => { };
    }
}