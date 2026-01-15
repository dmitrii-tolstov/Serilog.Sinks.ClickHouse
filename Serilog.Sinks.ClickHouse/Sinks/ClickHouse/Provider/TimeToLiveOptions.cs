using System.Collections.Generic;

namespace Serilog.Sinks.ClickHouse.Provider
{
    /// <summary>
    /// Attribute to determine time-to-live for column or table, for example "TTL timestamp + INTERVAL 1 MONTH"
    /// </summary>
    public class TimeToLiveOptions
    {
        /// <summary>
        /// Time-to-live for whole table
        /// </summary>
        public TimeToLiveAttribute Table { get; set; }

        /// <summary>
        /// Interval value
        /// </summary>
        public IDictionary<string,TimeToLiveAttribute> Fields { get; set; }
    }
}