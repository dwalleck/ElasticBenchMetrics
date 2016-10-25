using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticBenchMetrics.ViewModels
{
    public class ResultSummary
    {
        public string MedianTime { get; set; }

        public string NinetyFifthPercentileTime { get; set; }

        public string FailurePercentage { get; set; }
    }
}
