using Humanizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticBenchMetrics.ViewModels
{
    public class ResultSummary
    {
        public double RawMedianTime { get; set; }

        public double MedianTimeThreshold { get; set; }

        public string MedianTimeThresholdFormatted() => TimeSpan.FromSeconds(MedianTimeThreshold).Humanize(2);

        public string MedianTime() => TimeSpan.FromSeconds(RawMedianTime).Humanize(2);
        
        public string MedianThresholdStatus() => RawMedianTime < MedianTimeThreshold ? "success" : "danger";

        public double RawNinetyFifthPercentileTime { get; set; }

        public double NinetyFifthPercentileTimeThreshold  { get; set; }

        public string NinetyFifthPercentileTimeThresholdFormatted() => TimeSpan.FromSeconds(NinetyFifthPercentileTimeThreshold).Humanize(2);

        public string NinetyFifthPercentileTime() => TimeSpan.FromSeconds(RawNinetyFifthPercentileTime).Humanize(2);

        public string NinetyFifthPercentileStatus() => RawNinetyFifthPercentileTime < NinetyFifthPercentileTimeThreshold ? "success" : "danger";

        public double RawFailurePercentage { get; set; }

        public double FailurePercentageThreshold { get; set; }

        public string FailurePercentageThresholdFormatted() => FailurePercentageThreshold.ToString("F");

        public string FailurePercentage() => RawFailurePercentage.ToString("F");

        public string FailureThresholdStatus() => RawFailurePercentage < FailurePercentageThreshold ? "success" : "danger";
    }
}
