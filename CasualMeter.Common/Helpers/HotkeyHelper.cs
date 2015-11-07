using System;
using System.Windows.Forms;
using CasualMeter.Common.Conductors;
using Gma.UserActivityMonitor;

namespace CasualMeter.Common.Helpers
{
    public class HotkeyHelper
    {
        private static readonly Lazy<HotkeyHelper> Lazy = new Lazy<HotkeyHelper>(() => new HotkeyHelper());

        public static HotkeyHelper Instance
        {
            get { return Lazy.Value; }
        }

        private HotkeyHelper()
        {
        }

        public void Initialize()
        {
            HookManager.KeyDown += HookManagerOnKeyDown;
            HookManager.KeyUp += HookManagerOnKeyUp;
        }

        private void HookManagerOnKeyDown(object sender, KeyEventArgs e)
        {
            
        }

        private void HookManagerOnKeyUp(object sender, KeyEventArgs e)
        {
            
        }
    }
}
