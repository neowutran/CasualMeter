using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CasualMeter.Common.Conductors.Messages
{
    public class ResetPlayerStatsMessage
    {
        public bool ShouldSaveCurrent { get; set; }
    }
}
