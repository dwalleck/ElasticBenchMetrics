using Nest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticBenchMetrics.Models
{
    [ElasticsearchType(Name = "elasic_benchmark_result")]
    public class BenchmarkResult
    {
        [String(Name = "logs")]
        public string LogUrl { get; set; }

        [String(Name = "result")]
        public string Result { get; set; }

        [String(Name = "run_at")]
        public DateTime RunAt { get; set; }

        [String(Name = "run_id")]
        public string RunId { get; set; }

        [String(Name = "runtime")]
        public double TotalRuntime { get; set; }

        [JsonProperty("atomic_actions")]
        public Dictionary<string, double> AtomicActions { get; set; }

        [String(Name = "test_type")]
        public string TestType { get; set; }

        [String(Name = "scenario_name")]
        public string ScenarioName { get; set; }

    }
}
