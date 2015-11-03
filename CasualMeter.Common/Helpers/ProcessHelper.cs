using System;
using Lunyx.Common;

namespace CasualMeter.Common.Helpers
{
    public sealed class ProcessHelper
    {
        private static readonly Lazy<ProcessHelper> Lazy = new Lazy<ProcessHelper>(() => new ProcessHelper());

        public static ProcessHelper Instance { get { return Lazy.Value; } }

        private ProcessHelper() 
        {
        }

        public bool IsTeraActive { get { return ProcessInfo.GetActiveProcessName().Equals("Tera", StringComparison.OrdinalIgnoreCase); } }
    }
}
