using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nest;
using ElasticBenchMetrics.Models;
using Humanizer;
using ElasticBenchMetrics.ViewModels;
using Microsoft.Extensions.Configuration;
using ElasticBenchMetrics.Configuration;
using Microsoft.Extensions.Options;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ElasticBenchMetrics.Controllers
{
    public class MetricsController : Controller
    {
        public ElasticClient _client { get; set; }

        public MetricsController(IConfigurationRoot configuration)
        {
            var elasticSearchUrl = configuration["ElasticSearch:Host"];
            var indexName = configuration["ElasticSearch:IndexName"];
            var settings = new ConnectionSettings(new Uri(elasticSearchUrl))
                .DefaultIndex(indexName);
            settings.BasicAuthentication(
                configuration["ElasticSearch:Username"],
                configuration["ElasticSearch:Password"]);
            _client = new ElasticClient(settings);  
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            var lastDay = -5;
            var lastMonth = -30;
            var timePeriods = new List<int> { lastDay, lastMonth };
            var resultHistories = new List<ResultSummary>();
            var metricsOverview = new MetricsOverview
            {
                CinderCreateVolume = new ResultHistory(),
                CinderDeleteVolume = new ResultHistory(),
                GlanceCreateImage = new ResultHistory(),
                GlanceDeleteImage = new ResultHistory(),
                KeystoneAuthenticateStress = new ResultHistory(),
                NovaBootServer = new ResultHistory(),
                NovaBootServerStress = new ResultHistory()
            };
            foreach (var period in timePeriods)
            {
                var response = _client.Search<BenchmarkResult>(s => s
                    .Size(10000)
                    .Query(q => q
                        .DateRange(d => d
                            .Field(f => f.RunAt).GreaterThanOrEquals(DateTime.Today.AddDays(period))))
                    
                );

                var results = response.Documents;
                var resultsByScenario = results.GroupBy(r => r.ScenarioName);
                
                foreach (var resultGroup in resultsByScenario)
                {
                    var scenarioName = resultGroup.Key;
                    if (scenarioName == "NovaServers.boot_and_delete_server")
                    {
                        // Filter for only benchmarks
                        var scenarios = resultGroup.Where(r => r.TestType == "benchmark");

                        var sortedBootTimes = scenarios.OrderBy(o => o.AtomicActions["nova:boot_server"]).ToList();
                        var medianBootTime = TimeSpan.FromSeconds(sortedBootTimes.ElementAt(sortedBootTimes.Count / 2).AtomicActions["nova:boot_server"]);
                        var outlierBootTimeIndex = (int)(Math.Ceiling(sortedBootTimes.Count * 0.95) - 1);
                        var outlierBootTime = TimeSpan.FromSeconds(sortedBootTimes.ElementAt(outlierBootTimeIndex).AtomicActions["nova:boot_server"]);
                        var failurePercentage = (scenarios.Where(s => s.Result == "fail").Count() * 100.0) / scenarios.Count();

                        var summary = new ResultSummary
                        {
                            MedianTime = medianBootTime.Humanize(2),
                            MedianThresholdStatus = "success",
                            NinetyFifthPercentileTime = outlierBootTime.Humanize(2),
                            NinetyFifthPercentileStatus = "success",
                            FailurePercentage = failurePercentage.ToString("F"),
                            FailureThresholdStatus = "success"
                        };
                        if (period == lastDay)
                        {
                            metricsOverview.NovaBootServer.LastDay = summary;
                        }
                        else
                        {
                            metricsOverview.NovaBootServer.LastMonth = summary;
                        }
                        
                    }
                    else if (scenarioName == "NovaServers.boot_and_delete_server_stress")
                    {
                        // Filter for only stress tests
                        var stressScenarios = resultGroup.Where(r => r.TestType == "stress_test");
                        var sortedScenarioTimes = stressScenarios.OrderBy(o => o.TotalRuntime).ToList();
                        var medianScenarioTime = TimeSpan.FromSeconds(sortedScenarioTimes.ElementAt(sortedScenarioTimes.Count() / 2).TotalRuntime);
                        var scenarioFailurePercentage = (stressScenarios.Where(s => s.Result == "fail").Count() * 100.0) / stressScenarios.Count();
                        var outlierBootTimeIndex = (int)(Math.Ceiling(sortedScenarioTimes.Count * 0.95) - 1);
                        var outlierBootTime = TimeSpan.FromSeconds(sortedScenarioTimes.ElementAt(outlierBootTimeIndex).TotalRuntime);

                        var summary = new ResultSummary
                        {
                            MedianTime = medianScenarioTime.Humanize(2),
                            NinetyFifthPercentileTime = outlierBootTime.Humanize(2),
                            FailurePercentage = scenarioFailurePercentage.ToString("F"),
                            MedianThresholdStatus = "success",
                            NinetyFifthPercentileStatus = "success",
                            FailureThresholdStatus = "success"
                        };

                        if (period == lastDay)
                        {
                            metricsOverview.NovaBootServerStress.LastDay = summary;
                        }
                        else
                        {
                            metricsOverview.NovaBootServerStress.LastMonth = summary;
                        }
                    }
                    else if (scenarioName == "GlanceImages.create_and_delete_image")
                    {
                        // Filter for only benchmarks
                        var scenarios = resultGroup.Where(r => r.TestType == "benchmark");

                        var sortedBootTimes = scenarios.OrderBy(o => o.AtomicActions["glance:create_image"]).ToList();
                        var medianBootTime = TimeSpan.FromSeconds(sortedBootTimes.ElementAt(sortedBootTimes.Count / 2).AtomicActions["glance:create_image"]);
                        var outlierBootTimeIndex = (int)(Math.Ceiling(sortedBootTimes.Count * 0.95) - 1);
                        var outlierBootTime = TimeSpan.FromSeconds(sortedBootTimes.ElementAt(outlierBootTimeIndex).AtomicActions["glance:create_image"]);
                        var failurePercentage = (scenarios.Where(s => s.Result == "fail").Count() * 100.0) / scenarios.Count();

                        var summary = new ResultSummary
                        {
                            MedianTime = medianBootTime.Humanize(2),
                            NinetyFifthPercentileTime = outlierBootTime.Humanize(2),
                            FailurePercentage = failurePercentage.ToString("F"),
                            MedianThresholdStatus = "success",
                            NinetyFifthPercentileStatus = "success",
                            FailureThresholdStatus = "success"
                        };

                        if (period == lastDay)
                        {
                            metricsOverview.GlanceCreateImage.LastDay = summary;
                        }
                        else
                        {
                            metricsOverview.GlanceCreateImage.LastMonth = summary;
                        }

                    }
                    else if (scenarioName == "CinderVolumes.create_and_delete_volume")
                    {
                        // Filter for only benchmarks
                        var scenarios = resultGroup.Where(r => r.TestType == "benchmark");

                        var sortedBootTimes = scenarios.OrderBy(o => o.AtomicActions["cinder:create_volume"]).ToList();
                        var medianBootTime = TimeSpan.FromSeconds(sortedBootTimes.ElementAt(sortedBootTimes.Count / 2).AtomicActions["cinder:create_volume"]);
                        var outlierBootTimeIndex = (int)(Math.Ceiling(sortedBootTimes.Count * 0.95) - 1);
                        var outlierBootTime = TimeSpan.FromSeconds(sortedBootTimes.ElementAt(outlierBootTimeIndex).AtomicActions["cinder:create_volume"]);
                        var failurePercentage = (scenarios.Where(s => s.Result == "fail").Count() * 100.0) / scenarios.Count();

                        var createSummary = new ResultSummary
                        {
                            MedianTime = medianBootTime.Humanize(2),
                            NinetyFifthPercentileTime = outlierBootTime.Humanize(2),
                            FailurePercentage = failurePercentage.ToString("F"),
                            MedianThresholdStatus = "success",
                            NinetyFifthPercentileStatus = "success",
                            FailureThresholdStatus = "success"
                        };

                        var sortedDeletedTimes = scenarios.OrderBy(o => o.AtomicActions["cinder:delete_volume"]).ToList();
                        var medianDeleteTime = TimeSpan.FromSeconds(sortedDeletedTimes.ElementAt(sortedDeletedTimes.Count / 2).AtomicActions["cinder:delete_volume"]);
                        var outlierDeleteTimeIndex = (int)(Math.Ceiling(sortedDeletedTimes.Count * 0.95) - 1);
                        var outlierDeleteTime = TimeSpan.FromSeconds(sortedDeletedTimes.ElementAt(outlierDeleteTimeIndex).AtomicActions["cinder:delete_volume"]);
                        var failureDeletePercentage = (scenarios.Where(s => s.Result == "fail").Count() * 100.0) / scenarios.Count();

                        var deleteSummary = new ResultSummary
                        {
                            MedianTime = medianDeleteTime.Humanize(2),
                            NinetyFifthPercentileTime = outlierDeleteTime.Humanize(2),
                            FailurePercentage = failureDeletePercentage.ToString("F"),
                            MedianThresholdStatus = "success",
                            NinetyFifthPercentileStatus = "success",
                            FailureThresholdStatus = "success"
                        };

                        if (period == lastDay)
                        {
                            metricsOverview.CinderCreateVolume.LastDay = createSummary;
                            metricsOverview.CinderDeleteVolume.LastDay = deleteSummary;
                        }
                        else
                        {
                            metricsOverview.CinderCreateVolume.LastMonth = createSummary;
                            metricsOverview.CinderDeleteVolume.LastMonth = deleteSummary;
                        }
                    }
                    else if (scenarioName == "Authenticate.keystone")
                    {
                        // Filter for only stress tests
                        var stressScenarios = resultGroup.Where(r => r.TestType == "stress_test");
                        var sortedScenarioTimes = stressScenarios.OrderBy(o => o.TotalRuntime).ToList();
                        var medianScenarioTime = TimeSpan.FromSeconds(sortedScenarioTimes.ElementAt(sortedScenarioTimes.Count() / 2).TotalRuntime);
                        var scenarioFailurePercentage = (stressScenarios.Where(s => s.Result == "fail").Count() * 100.0) / stressScenarios.Count();
                        var outlierScenarioTimeIndex = (int)(Math.Ceiling(sortedScenarioTimes.Count * 0.95) - 1);
                        var outlierScenarioTime = TimeSpan.FromSeconds(sortedScenarioTimes.ElementAt(outlierScenarioTimeIndex).TotalRuntime);

                        var summary = new ResultSummary
                        {
                            MedianTime = medianScenarioTime.Humanize(2),
                            NinetyFifthPercentileTime = outlierScenarioTime.Humanize(2),
                            FailurePercentage = scenarioFailurePercentage.ToString("F"),
                            MedianThresholdStatus = "success",
                            NinetyFifthPercentileStatus = "success",
                            FailureThresholdStatus = "success"
                        };

                        if (period == lastDay)
                        {
                            metricsOverview.KeystoneAuthenticateStress.LastDay = summary;
                        }
                        else
                        {
                            metricsOverview.KeystoneAuthenticateStress.LastMonth = summary;
                        }
                    }
                    else
                    {
                        continue;
                    }
                  }

            }

            return View(metricsOverview);
        }
    }
}
