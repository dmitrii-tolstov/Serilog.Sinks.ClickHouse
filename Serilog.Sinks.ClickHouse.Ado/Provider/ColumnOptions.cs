using System.Collections.Generic;

namespace Serilog.Sinks.ClickHouse.Ado.Provider
{
    public class ColumnOptions
    {
        public IEnumerable<string> RemoveStandardColumns { get; set; }

        /// <summary>
        /// Field name collection for ORDER BY
        /// IF null - timestamp. IF not null and empty - tuple().
        /// </summary>
        public IEnumerable<string> OrderBy { get; set; }

        /// <summary>
        /// Field name collection for PARTITION BY
        /// IF null - timestamp. IF not null and empty - tuple().
        /// </summary>
        public IEnumerable<string> PartitionBy { get; set; }

        /// <summary>
        /// Time-to-live options for table or columns
        /// </summary>
        public TimeToLiveOptions TimeToLive { get; set; }
    }
}