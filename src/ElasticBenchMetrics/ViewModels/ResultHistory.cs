using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticBenchMetrics.ViewModels
{
    public class ResultHistory
    {
        public ResultSummary LastDay { get; set; }

        public ResultSummary LastMonth { get; set; }
    }
}
