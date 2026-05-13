using System;
using System.Collections.Generic;
using System.Linq;

namespace Serilog.Sinks.ClickHouse.Provider
{
    class TableHelper<T>
    {
        public string Create { get; private set; }
        public string Insert { get; private set; }

        public TableHelper(string name, IEnumerable<AdditionalColumn> additionalColumns = null, ColumnOptions columnOptions = null)
        {
            var mapping = ColumnsHelper.Mapping<T>();

            if (columnOptions?.RemoveStandardColumns != null)
            {
                mapping.RemoveAll(x => columnOptions.RemoveStandardColumns.Contains(x.Name));
            }

            if (additionalColumns != null)
                mapping = mapping.Union(additionalColumns.Select(c => new ColumnAttribute { Name = c.Name, Type = c.Type })).ToList();

            if (!TimeToLiveHelper.ValidateOptions(columnOptions?.TimeToLive, mapping, "TimeToLive."))
            {
                throw new ArgumentException("Wrong section: TimeToLive");
            }

            Create = $@"CREATE TABLE IF NOT EXISTS {name} (
                    {string.Join(", ", mapping.Select(m => $"{m.Name} {m.Type} { TimeToLiveHelper.GetScript((columnOptions?.TimeToLive?.Fields.ContainsKey(m.Name) == true) ? columnOptions?.TimeToLive?.Fields[m.Name] : null)}"))}
                )
                ENGINE = MergeTree()
                {TableOrderByHelper.GetScript(columnOptions?.OrderBy)}
                {TablePartitionByHelper.GetScript(columnOptions?.PartitionBy)}
                {TimeToLiveHelper.GetScript(columnOptions?.TimeToLive?.Table)}
                ";

            Insert = $@"INSERT INTO {name} (
                    {string.Join(", ", mapping.Select(m => $"{m.Name}"))}
                ) VALUES @bulk";
        }
    }
}
