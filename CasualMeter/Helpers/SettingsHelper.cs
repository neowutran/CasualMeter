using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CasualMeter.Helpers
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
        }

        public bool IsTabVisibilityEnabled { get; set; }
    }
}
