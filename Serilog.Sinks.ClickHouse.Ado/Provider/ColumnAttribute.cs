using System;

namespace Serilog.Sinks.ClickHouse.Ado.Provider
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute : Attribute
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }
}
