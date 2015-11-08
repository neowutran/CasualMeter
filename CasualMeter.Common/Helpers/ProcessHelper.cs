using System;
using Lunyx.Common;

namespace CasualMeter.Common.Helpers
{
    public sealed class ProcessHelper
    {
        private static readonly Lazy<ProcessHelper> Lazy = new Lazy<ProcessHelper>(() => new ProcessHelper());

        public static ProcessHelper Instance => Lazy.Value;

        private ProcessHelper() 
        {
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
