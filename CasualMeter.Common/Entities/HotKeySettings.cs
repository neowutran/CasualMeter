using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CasualMeter.Common.Helpers;

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

        public string Modifier { get; set; }
        public string PasteStats { get; set; }
        public string Reset { get; set; }
        public string SaveAndReset { get; set; }
    }
}
