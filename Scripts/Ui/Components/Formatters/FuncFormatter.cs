﻿using System;
using UnityEngine;
using UnityEngine.UI;

namespace _Framework.Scripts.Ui.Components.Formatters
{
    public class FuncFormatter : ValueFormatter
    {
        [SerializeField] private Text label;
        
        public Func<object, string> Formatter { get; set; }

        public override void OnValueChanged(object o) => label.text = Formatter(o);
    }
}