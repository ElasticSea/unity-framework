using System;
using Core.Ui.Binding;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Ui.Components
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