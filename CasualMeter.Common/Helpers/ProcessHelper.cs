using System;
using System.Windows.Media;
using CasualMeter.Common.Conductors;
using Lunyx.Common;

namespace CasualMeter.Common.Helpers
{
    public sealed class ProcessHelper
    {
        private static readonly Lazy<ProcessHelper> Lazy = new Lazy<ProcessHelper>(() => new ProcessHelper());

        public static ProcessHelper Instance => Lazy.Value;

        private ProcessInfo.WinEventDelegate dele;//leave this here to prevent garbage collection

        private ProcessHelper() 
        {
            //listen to window focus changed event
            dele = (OnFocusedWindowChanged);
            ProcessInfo.RegisterWindowFocusEvent(dele);
        }

        public void Initialize()
        {
            //empty method to ensure initialization
        }

        public void ForceVisibilityRefresh()
        {
            CasualMessenger.Instance.RefreshVisibility(IsTeraActive);
        }

        private void OnFocusedWindowChanged(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            ForceVisibilityRefresh();
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

        public bool IsTeraActive => ProcessInfo.GetActiveProcessName().Equals("Tera", StringComparison.OrdinalIgnoreCase);
        public IntPtr TeraWindow => ProcessInfo.FindWindow("LaunchUnrealUWindowsClient", "TERA");
    }
}
