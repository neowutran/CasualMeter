using System;

namespace CasualMeter.Common.Helpers
{
    public sealed class SettingsHelper
    {
        private static readonly Lazy<SettingsHelper> Lazy = new Lazy<SettingsHelper>(() => new SettingsHelper());
        
        public static SettingsHelper Instance
        {
            get { return Lazy.Value; }
        }

        private SettingsHelper()
        {
        }

        public void Initialize()
        {
            IsTabVisibilityEnabled = true;
            ServerIp = "208.67.49.28";
            LastActiveCaptureDeviceName = "Wi-Fi 2";
        }

        public bool IsTabVisibilityEnabled { get; set; }
        public string ServerIp { get; set; }
        public string LastActiveCaptureDeviceName { get; set; }
    }
}
