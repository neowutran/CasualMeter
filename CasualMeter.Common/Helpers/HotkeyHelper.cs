using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CasualMeter.Common.Conductors;
using CasualMeter.Common.Entities;
using Gma.UserActivityMonitor;

namespace CasualMeter.Common.Helpers
{
    public class HotkeyHelper
    {
        private static readonly Lazy<HotkeyHelper> Lazy = new Lazy<HotkeyHelper>(() => new HotkeyHelper());

        private readonly Dictionary<Keys, Keys[]> _modifierMappings = new Dictionary<Keys, Keys[]>
        {
            {Keys.Control, new [] {Keys.Control, Keys.ControlKey, Keys.LControlKey, Keys.RControlKey} },
            {Keys.Alt, new [] {Keys.Alt} },
            {Keys.Shift, new [] {Keys.Shift, Keys.ShiftKey, Keys.LShiftKey, Keys.RShiftKey} }
        };

        private readonly Dictionary<Keys, Action> _keyActionMappings;

        private HotKeySettings HotKeys => SettingsHelper.Instance.Settings.HotKeys;
     
        public static HotkeyHelper Instance => Lazy.Value;

        private HotkeyHelper()
        {
            HookManager.KeyDown += HookManagerOnKeyDown;
            HookManager.KeyUp += HookManagerOnKeyUp;

            _keyActionMappings = new Dictionary<Keys, Action>()
            {
                {HotKeys.PasteStats.AsKeys(), CasualMessenger.Instance.PastePlayerStats},
                {HotKeys.Reset.AsKeys(), () => CasualMessenger.Instance.ResetPlayerStats(false)},
                {HotKeys.SaveAndReset.AsKeys(), () => CasualMessenger.Instance.ResetPlayerStats(true)}
            };
        }

        public void Initialize()
        {
            //empty method to ensure initialization
        }

        private volatile bool _isModifierPressed;

        private void HookManagerOnKeyDown(object sender, KeyEventArgs e)
        {
            if (_modifierMappings.Any(k => k.Value.Contains(e.KeyCode)))
                _isModifierPressed = ModifierKeyPressed(e.KeyCode);

            if (_isModifierPressed || HotKeys.Modifier.AsKeys() == Keys.None)
            {
                if (_keyActionMappings.ContainsKey(e.KeyCode) && _keyActionMappings[e.KeyCode] != null && ProcessHelper.Instance.IsTeraActive)
                    _keyActionMappings[e.KeyCode]();
            }
        }

        private void HookManagerOnKeyUp(object sender, KeyEventArgs e)
        {
            if (_modifierMappings.Any(k => k.Value.Contains(e.KeyCode)))
                _isModifierPressed = !ModifierKeyPressed(e.KeyCode);
        }

        private bool ModifierKeyPressed(Keys keyCode)
        {
            var modifier = _modifierMappings.FirstOrDefault(mm => mm.Value.Any(m => m == HotKeys.Modifier.AsKeys())).Key;

            switch (modifier)
            {
                case Keys.Control:
                    return _modifierMappings[Keys.Control].Contains(keyCode);
                case Keys.Alt:
                    return _modifierMappings[Keys.Alt].Contains(keyCode);
                case Keys.Shift:
                    return _modifierMappings[Keys.Shift].Contains(keyCode);
                default:
                    return false;
            }
        }
    }

    public static class HotKeyExtensions
    {
        public static string AsString(this Keys key)
        {
            return (String)new KeysConverter().ConvertTo(key, typeof(string));
        }

        public static Keys AsKeys(this string s)
        {
            if (s == null)
                return Keys.None;
            return ((Keys?)new KeysConverter().ConvertFrom(s)) ?? Keys.None;
        }
    }
}
