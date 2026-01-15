using System;
using System.Collections.Generic;
using System.Linq;
using Serilog.Debugging;

namespace Serilog.Sinks.ClickHouse.Provider
{
    /// <summary>
    /// Attribute to determine time-to-live for column or table, for example "TTL timestamp + INTERVAL 1 MONTH"
    /// </summary>
    public class TimeToLiveHelper
    {
        protected static readonly List<string> AvailableIntervalNames = ["YEAR", "QUARTER", "MONTH", "WEEK", "DAY", "HOUR", "MINUTE", "SECOND", "MILLISECOND", "MICROSECOND", "NANOSECOND"];

        /// <summary>
        /// Get TTL script for TimeToLiveAttribute
        /// </summary>
        public static string GetScript(TimeToLiveAttribute timeToLiveAttribute)
        {
            string ret = "";

            if (timeToLiveAttribute != null)
            {
                ret = $"TTL {timeToLiveAttribute.DateTimeColumnName} + INTERVAL {timeToLiveAttribute.IntervalValue} {timeToLiveAttribute.IntervalName}";
            }

            return ret;
        }

        protected static bool ValidateAttribute(TimeToLiveAttribute timeToLiveAttribute, List<ColumnAttribute> columns, string errorMessagePrefix)
        {
            bool ret = true;

            if (timeToLiveAttribute is null)
            {
                SelfLog.WriteLine($"{errorMessagePrefix}: {timeToLiveAttribute} is null");
                return false;
            }

            if (!columns.Any(s => s.Name.Equals(timeToLiveAttribute.DateTimeColumnName,StringComparison.OrdinalIgnoreCase)))
            {
                SelfLog.WriteLine($"{errorMessagePrefix}: field with name {timeToLiveAttribute.DateTimeColumnName} does not exist");
                ret = false;
            }

            if (!AvailableIntervalNames.Any(a => a.Equals(timeToLiveAttribute.IntervalName, StringComparison.OrdinalIgnoreCase)))
            {
                SelfLog.WriteLine($"{errorMessagePrefix}: interval name {timeToLiveAttribute.IntervalName} is not supported");
                ret = false;
            }

            return ret;
        }

        public static bool ValidateOptions(TimeToLiveOptions timeToLiveOptions, List<ColumnAttribute> columns, string errorMessagePrefix)
        {
            bool ret = true;

            if (timeToLiveOptions is null)
            {
                return true;
            }

            ret = ValidateAttribute(timeToLiveOptions.Table,columns,errorMessagePrefix + ".Table") && ret;

            if (timeToLiveOptions.Fields != null)
            {
                foreach (var field in timeToLiveOptions.Fields)
                {
                    var fieldName = field.Key;
                    var attribute = field.Value;

                    var errorMessagePrefixLocal = errorMessagePrefix + ".Fields." + fieldName;

                    if (!columns.Any(s => s.Name.Equals(fieldName,StringComparison.OrdinalIgnoreCase)))
                    {
                        SelfLog.WriteLine($"{errorMessagePrefixLocal}: field with name {fieldName} does not exist");
                        ret = false;
                    }

                    ret = ValidateAttribute(attribute,columns,errorMessagePrefixLocal) && ret;
                }
            }

            return ret;
        }
    }
}