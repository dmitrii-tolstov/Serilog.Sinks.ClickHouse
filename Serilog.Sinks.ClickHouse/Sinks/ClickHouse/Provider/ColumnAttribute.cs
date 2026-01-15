using System;

namespace Serilog.Sinks.ClickHouse.Provider
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute : Attribute
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }
}
