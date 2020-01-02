using System;
using System.Collections.Generic;
using System.Text;

namespace Polly.Contrib.CachePolicy.Providers.Logging
{
    public interface IOperationalMetricLogger
    {
        /// <summary>
        /// Logs the given metric
        /// </summary>
        void LogMetric(string metricName, long metricValue);

        /// <summary>
        /// Logs the given metric
        /// </summary>
        /// <param name="metricName">Metric name</param>
        /// <param name="dimensions">
        /// Dimensions of the metric - Properties which would give us a deeper insight into what the metric is about and 
        /// by which we would be interested to slice and dice the metric to get a more meaningful information.
        /// 
        /// For example, let us say we are logging the 'Processor Utilization Percentage' metric. It would be useful to have 'EnvironmentName' (Name of a datacenter etc.),
        /// 'InstanceType' (WorkerRoleA, WebRoleB etc.) and 'Instance' (Name, ID etc. of the VM) as some of the dimensions.
        /// These dimensions would help us to get data like, 'Average CPU Utilization Percentage for a DataCenter across InstanceTypes and Instances',
        /// 'Average CPU Utilization Percentage for a particular InstanceType across Datacenters', 'Average CPU Utilization Percentage for a specific VM in a specific Datacenter' etc.
        /// 
        /// Dimensions help us to get different 'views' of the same metric.
        /// Note: Be careful not to log PII data as part of dimensions.
        /// </param>
        /// <param name="metricValue">Actual value of the metric</param>
        void LogMetric(string metricName, Dictionary<string, string> dimensions, long metricValue);
    }
}
