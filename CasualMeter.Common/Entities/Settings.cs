using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CasualMeter.Common.Entities
{
    public class Settings
    {
        public Settings() : this(100, 100, new HotKeySettings(), 1, 1)
        {

        }

        public Settings(double windowLeft, double windowTop, HotKeySettings hotKeys,
            double opacity, double uiScale)
        {
            WindowLeft = windowLeft;
            WindowTop = windowTop;
            HotKeys = hotKeys;
            Opacity = opacity;
            UiScale = uiScale;
        }

        [DefaultValue(100)]
        public double WindowLeft { get; set; }

        [DefaultValue(100)]
        public double WindowTop { get; set; }

        [DefaultValue(1)]
        public double Opacity { get; set; }

        [DefaultValue(1)]
        public double UiScale { get; set; }

        //since you can't set DefaultValueAttribute on objects
        private HotKeySettings _hotkeys;
        public HotKeySettings HotKeys
        {
            get { return _hotkeys ?? (_hotkeys = new HotKeySettings()); }
            set { _hotkeys = value; }
        }
    }
}
