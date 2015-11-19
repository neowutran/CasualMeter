using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using CasualMeter.Common.Conductors;
using CasualMeter.Common.Entities;
using GlobalHotKey;

namespace CasualMeter.Common.Helpers
{
    public class HotkeyHelper
    {
        private static readonly Lazy<HotkeyHelper> Lazy = new Lazy<HotkeyHelper>(() => new HotkeyHelper());
        public static HotkeyHelper Instance => Lazy.Value;

        private HotKeySettings HotKeys
        {
            get { return SettingsHelper.Instance.Settings.HotKeys; }
            set { SettingsHelper.Instance.Settings.HotKeys = value; }
        }

        private readonly HotKeyManager _manager;
        private readonly Dictionary<HotKey, Action> _actions;

        private HotkeyHelper()
        {
            _manager = new HotKeyManager();
            _actions = new Dictionary<HotKey, Action>();
            _manager.KeyPressed += Manager_KeyPressed;
        }

        public void Initialize()
        {
            Register(HotKeys.Modifier, HotKeys.PasteStats, CasualMessenger.Instance.PastePlayerStats);
            Register(HotKeys.Modifier, HotKeys.Reset, () => CasualMessenger.Instance.ResetPlayerStats(false));
            Register(HotKeys.Modifier, HotKeys.SaveAndReset, () => CasualMessenger.Instance.ResetPlayerStats(true));
        }

        private void Manager_KeyPressed(object sender, KeyPressedEventArgs e)
        {
            if (e.HotKey != null)
                _actions[e.HotKey]();
        }

        public void Register(ModifierKeys modKey, Key key, Action action)
        {
            var hotkey = new HotKey(key, modKey);
            if (_actions.ContainsKey(hotkey))
                throw new ArgumentException("Hotkey already registered!");
            _actions[hotkey] = action;

            _manager.Register(hotkey);
        }

        public void Unregister(ModifierKeys modKey, Key key)
        {
            var hotkey = new HotKey(key, modKey);
            if (_actions.ContainsKey(hotkey))
            {
                _manager.Unregister(hotkey);
                _actions.Remove(hotkey);
            }
        }
    }
}
