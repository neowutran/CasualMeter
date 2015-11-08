using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CasualMeter.Common.Entities
{
    public class Settings
    {
        public Settings() : this(100, 100, new HotKeySettings())
        {

        }

        public Settings(double windowLeft, double windowTop, HotKeySettings hotKeys)
        {
            WindowLeft = windowLeft;
            WindowTop = windowTop;
            HotKeys = hotKeys;
        }

        public double WindowLeft { get; set; }
        public double WindowTop { get; set; }

        public HotKeySettings HotKeys { get; set; }
    }
}
