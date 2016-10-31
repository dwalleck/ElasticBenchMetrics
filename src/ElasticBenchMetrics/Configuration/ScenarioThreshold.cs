using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticBenchMetrics.Configuration
{

    public class ScenarioThresholds
    {
        public ScenarioThresholds()
        {

        }
        public List<ScenarioThreshold> RallyScenarioThresholds { get; set; }
    }

    public class ScenarioThreshold
    {
        public ScenarioThreshold()
        {

        }
        public string Name { get; set; }

        public double ExecutionTime { get; set; }

        public double FailureRate { get; set; }

        public Dictionary<string, double> AtomicActions { get; set; }
    }
}
