using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticBenchMetrics.ViewModels
{
    public class MetricsOverview
    {
        public ResultHistory NovaBootServer { get; set; }

        public ResultHistory NovaBootServerStress { get; set; }

        public ResultHistory CinderCreateVolume { get; set; }

        public ResultHistory CinderDeleteVolume { get; set; }

        public ResultHistory KeystoneAuthenticateStress { get; set; }

        public ResultHistory GlanceCreateImage { get; set; }

        public ResultHistory GlanceDeleteImage { get; set; }
    }
}
