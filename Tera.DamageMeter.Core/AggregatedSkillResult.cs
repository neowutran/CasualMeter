using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tera.DamageMeter
{
    public class AggregatedSkillResult
    {
        public string DisplayName { get; set; }
        public long Amount { get; set; }
        public int Hits { get; set; }
        public double CritRate { get; set; }
        public long HighestCrit { get; set; }
        public long LowestCrit { get; set; }
        public long AverageCrit { get; set; }
        public long AverageWhite { get; set; }
        public long DamagePercent { get; set; }
    }
}
