using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using CasualMeter.Common.Conductors;
using CasualMeter.Common.Entities;
using GlobalHotKey;
using log4net;

namespace CasualMeter.Common.Helpers
{
    public class HotkeyHelper
    {
        private static readonly ILog Logger = LogManager.GetLogger
            (MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly Lazy<HotkeyHelper> Lazy = new Lazy<HotkeyHelper>(() => new HotkeyHelper());
        public static HotkeyHelper Instance => Lazy.Value;

        private HotKeySettings HotKeys
        {
            get { return SettingsHelper.Instance.Settings.HotKeys; }
            set { SettingsHelper.Instance.Settings.HotKeys = value; }
        }

        private readonly HotKeyManager _manager;
        private readonly Dictionary<HotKey, Action> _actions;
        private readonly object _lock;

        private HotkeyHelper()
        {
            _lock = new object();
            _manager = new HotKeyManager();
            _actions = new Dictionary<HotKey, Action>();
            _manager.KeyPressed += Manager_KeyPressed;
        }

        public void Initialize()
        {
            //empty method to ensure initialization
        }

        public void Activate()
        {
            lock (_lock)
            {
                Register(HotKeys.Modifier, HotKeys.PasteStats, CasualMessenger.Instance.PastePlayerStats);
                Register(HotKeys.Modifier, HotKeys.Reset, () => CasualMessenger.Instance.ResetPlayerStats(false));
                Register(HotKeys.Modifier, HotKeys.SaveAndReset, () => CasualMessenger.Instance.ResetPlayerStats(true));
            }
        }

        public void Deactivate()
        {   //don't do anything if we want to use hotkeys across all applications
            if (SettingsHelper.Instance.Settings.UseGlobalHotkeys) return;
            lock (_lock)
            {
                Unregister(HotKeys.Modifier, HotKeys.PasteStats);
                Unregister(HotKeys.Modifier, HotKeys.Reset);
                Unregister(HotKeys.Modifier, HotKeys.SaveAndReset);
            }
        }

        private void Manager_KeyPressed(object sender, KeyPressedEventArgs e)
        {
            if (e.HotKey != null)
                _actions[e.HotKey]();
        }

        private void Register(ModifierKeys modKey, Key key, Action action)
        {
            try
            {
                var hotkey = new HotKey(key, modKey);
                if (_actions.ContainsKey(hotkey))
                    return;
                _actions[hotkey] = action;

                _manager.Register(hotkey);
            }
            catch (Exception e)
            {
                Logger.Warn($"Failed to register the hotkey {modKey}+{key}. {e.Message}");
            }
        }

        private void Unregister(ModifierKeys modKey, Key key)
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
