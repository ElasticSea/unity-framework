using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using _Framework.Scripts.Extensions;

namespace _Framework.Scripts.Ui.Components
{
    public class ToggleGroup
    {
        // These are loaded after click
        public bool? PendingAllowMultiple;
        public bool? PendingAllowSwitchingOff;
        
        private bool allowMultiple;
        public bool AllowMultiple
        {
            get { return allowMultiple; }
            set
            {
                var originalValue = allowMultiple;
                allowMultiple = value;
                if (originalValue && value == false && Active.Count > 1)
                {
                    Active.ForEach(t => t.Selected = t == Active.First());
                    TriggerActiveTogglesChanged();
                }
            }
        }

        private bool allowSwitchingOff;
        public bool AllowSwitchingOff
        {
            get { return allowSwitchingOff; }
            set
            {
                var originalValue = allowSwitchingOff;
                allowSwitchingOff = value;
                if (originalValue && value == false && Active.Count == 0)
                {
                    Toggles.First().Selected = true;
                    TriggerActiveTogglesChanged();
                }
            }
        }

        private List<IToggle> toggles = new List<IToggle>();
        private (IToggle toggle, Action callback)[] callbacks = new (IToggle toggle, Action callback)[0];

        public ToggleGroup(bool allowMultiple = false, bool allowSwitchingOff = false)
        {
            AllowMultiple = allowMultiple;
            AllowSwitchingOff = allowSwitchingOff;
        }

        public List<IToggle> Toggles
        {
            get => toggles;
            set
            {
                // Remove listeners for existing toggles
                for (var i = 0; i < callbacks.Length; i++)
                {
                    var call = callbacks[i];
                    call.toggle.OnClick -= call.callback;
                }

                Debug.Assert(value != null, nameof(value) + " != null");

                toggles = value;

                callbacks = new (IToggle toggle, Action callback)[toggles.Count];
                for (var i = 0; i < Toggles.Count; i++)
                {
                    var toggle = Toggles[i];
                    var toggleOnOnClick = CreateClickCallback(toggle);
                    toggle.OnClick += toggleOnOnClick;
                    
                    callbacks[i] = (toggle, toggleOnOnClick);
                }
            }
        }

        private Action CreateClickCallback(IToggle toggle)
        {
            return () =>
            {
                if (PendingAllowMultiple != null)
                {
                    AllowMultiple = PendingAllowMultiple.Value;
                    PendingAllowMultiple = null;
                }
                        
                if (PendingAllowSwitchingOff != null)
                {
                    AllowSwitchingOff = PendingAllowSwitchingOff.Value;
                    PendingAllowSwitchingOff = null;
                }
                        
                var anySelected = Active.FirstOrDefault();
                if (anySelected != null)
                {
                    if (toggle.Selected)
                    {
                        if (AllowSwitchingOff || Active.Count > 1)
                        {
                            toggle.Selected = false;
                        }
                    }
                    else
                    {
                        if (AllowMultiple == false)
                        {
                            anySelected.Selected = false;
                        }

                        toggle.Selected = true;
                    }
                }
                else
                {
                    toggle.Selected = true;
                }

                TriggerActiveTogglesChanged();

                // TODO Move it elsewhere
                //                        SoundManager.Instance?.UISwitch();
            };
        }

        public ISet<IToggle> Active
        {
            get { return Toggles.Where(t => t.Selected).ToSet(); }
            set
            {
                if (value.Count > 1 && AllowMultiple == false)
                {
                    value = Toggles.Where(t => t.Selected).Take(1).ToSet();
//                    throw new InvalidOperationException("Toggle group does not support mutlitple active toggles.");
                }

                if (value.Count < 1 && AllowSwitchingOff == false)
                {
                    value = Toggles.Where(t => t.Selected).Take(1).ToSet();
//                    throw new InvalidOperationException("Toggle group does not support having all toggles turned off.");
                }

                foreach (var toggle in Toggles)
                {
                    toggle.Selected = value.Contains(toggle);
                }

                TriggerActiveTogglesChanged();
            }
        }

        public event Action<IEnumerable<IToggle>> OnActiveTogglesChanged = obj => { };

        private void TriggerActiveTogglesChanged() => OnActiveTogglesChanged(Active);
    }
}