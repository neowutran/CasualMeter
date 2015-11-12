using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CasualMeter.Common.Helpers;
using Nicenis.ComponentModel;

namespace CasualMeter.Common.Entities
{
    public class HotKeySettings
    {
        public HotKeySettings() : this(Keys.ControlKey.AsString(),
                                       Keys.Insert.AsString(),
                                       Keys.Delete.AsString(),
                                       Keys.End.AsString())
        {
            
        }

        public HotKeySettings(string modifier, string pasteStats, string reset, string saveAndReset)
        {
            Modifier = modifier;
            PasteStats = pasteStats;
            Reset = reset;
            SaveAndReset = saveAndReset;
        }

        [DefaultValue("ControlKey")]
        public string Modifier { get; set; }

        [DefaultValue("Ins")]
        public string PasteStats { get; set; }

        [DefaultValue("Del")]
        public string Reset { get; set; }

        [DefaultValue("End")]
        public string SaveAndReset { get; set; }
    }
}
