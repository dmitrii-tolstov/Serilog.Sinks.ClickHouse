namespace Serilog.Sinks.ClickHouse.Ado.Provider
{
    /// <summary>
    /// Attribute to determine time-to-live for column or table, for example "TTL timestamp + INTERVAL 1 MONTH"
    /// </summary>
    public class TimeToLiveAttribute
    {
        /// <summary>
        /// Base date or datetime column name
        /// </summary>
        public string DateTimeColumnName { get; set; }

        /// <summary>
        /// Interval value
        /// </summary>
        public int IntervalValue { get; set; }

        /// <summary>
        /// Interval name (MONTH, DAY, HOUR ...)
        /// </summary>
        public string IntervalName { get; set; }
    }
}