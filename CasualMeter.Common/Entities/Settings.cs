using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CasualMeter.Common.Entities
{
    public class Settings : DefaultValueEntity
    {
        [DefaultValue(100)]
        public double WindowLeft { get; set; }

        [DefaultValue(100)]
        public double WindowTop { get; set; }

        [DefaultValue(1)]
        public double Opacity { get; set; }

        [DefaultValue(1)]
        public double UiScale { get; set; }

        [DefaultValue(true)]
        public bool IsPinned { get; set; }

        [DefaultValue(false)]
        public bool UseCompactView { get; set; }

        [DefaultValue(false)]
        public bool ShowPersonalDps { get; set; }

        [DefaultValue("{Name} {DPS} {DamagePercent}")]
        public string DpsPasteFormat { get; set; }

        //since you can't set DefaultValueAttribute on objects
        private HotKeySettings _hotkeys;
        public HotKeySettings HotKeys
        {
            get { return _hotkeys ?? (_hotkeys = new HotKeySettings()); }
            set { _hotkeys = value; }
        }
    }
}
