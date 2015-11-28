using System;
using CasualMeter.Common.Conductors;
using Lunyx.Common;

namespace CasualMeter.Common.Helpers
{
    public sealed class ProcessHelper
    {
        private static readonly Lazy<ProcessHelper> Lazy = new Lazy<ProcessHelper>(() => new ProcessHelper());

        public static ProcessHelper Instance => Lazy.Value;

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly ProcessInfo.WinEventDelegate _dele;//leave this here to prevent garbage collection

        private ProcessHelper() 
        {
            //listen to window focus changed event
            _dele = OnFocusedWindowChanged;
            ProcessInfo.RegisterWindowFocusEvent(_dele);
        }

        public void Initialize()
        {
            //empty method to ensure initialization
        }

        public void ForceVisibilityRefresh()
        {
            CasualMessenger.Instance.RefreshVisibility(IsTeraActive);
        }

        public void UpdateHotKeys()
        {
            if (IsTeraActive)
                HotkeyHelper.Instance.Initialize();
            else
                HotkeyHelper.Instance.Deactivate();
        }

        private void OnFocusedWindowChanged(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            ForceVisibilityRefresh();
            UpdateHotKeys();
        }

        public bool SendString(string s)
        {
            if (TeraWindow == IntPtr.Zero)
                return false;
            try
            {
                ProcessInfo.SendString(TeraWindow, s);
                return true;
            }
            catch (Exception)
            {
                //eat this
            }
            return false;
        }

        public bool IsTeraActive
        {
            get
            {
                var processName = ProcessInfo.GetActiveProcessName();
                return processName.Equals("Tera", StringComparison.OrdinalIgnoreCase);//exception for screenshot application
            }
        }

        public IntPtr TeraWindow => ProcessInfo.FindWindow("LaunchUnrealUWindowsClient", "TERA");
    }
}
