using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Serilog.Events;

namespace Serilog.Sinks.ClickHouse.Ado.Provider
{
    class ColumnFormatter : IEnumerable
    {
        private static readonly Dictionary<PropertyInfo,ColumnAttribute> _props = ColumnsHelper.Props<ColumnFormatter>();

        private readonly LogEvent _message;
        private readonly IFormatProvider _formatProvider;
        private readonly IEnumerable<AdditionalColumn> _additionslColumns;
        private readonly IEnumerable<string> _removeStandardColumns;

        [Column(Name = "timestamp", Type = "DateTime")]
        public DateTime Timestamp { get => _message.Timestamp.UtcDateTime; }
        [Column(Name = "level", Type = "String")]
        public string Level { get => _message.Level.ToString(); }
        [Column(Name = "message", Type = "String")]
        public string Message { get => _message.RenderMessage(_formatProvider); }
        
        public ColumnFormatter(LogEvent message, IFormatProvider formatProvider = null, IEnumerable<AdditionalColumn> additionalColumns = null, IEnumerable<string> removeStandardColumns = null)
        {
            _message = message;
            _removeStandardColumns = removeStandardColumns;
            _additionslColumns = additionalColumns;
            _formatProvider = formatProvider;
        }

        public IEnumerator GetEnumerator()
        {
            foreach (var p in _props)
            {
                if (_removeStandardColumns is null || !_removeStandardColumns.Contains(p.Value.Name))
                {
                    yield return p.Key.GetValue(this);
                }
            }

            if (_additionslColumns != null)
                {
                    foreach (var col in _additionslColumns)
                    {
                        if (!_message.Properties.TryGetValue(col.Name, out var value))
                        {
                            yield return Default(col.Type);
                            continue;
                        }

                        if(!(value is ScalarValue scalarValue))
                        {
                            yield return value.ToString();
                            continue;
                        }

                        yield return scalarValue.Value;
                    }
                }
        }

        private object Default(string type)
        {
            switch (type)
            {
                case "Boolean": return default(bool);
                case "UInt": return default(uint);
                case "UInt8": return default(byte);
                case "UInt16": return default(UInt16);
                case "UInt32": return default(UInt32);
                case "UInt64": return default(UInt64);
                case "Int": return default(int);
                case "Int8": return default(sbyte);
                case "Int16": return default(Int16);
                case "Int32": return default(Int32);
                case "Int64": return default(Int64);
                case "Float32": return default(float);
                case "Float64": return default(double);
                case "Single": return default(float);
                case "Double": return default(double);
                case "DateTime": return default(DateTime);
                case "String": return default(string);
                default: throw new NotSupportedException();
            }
        }

    }
}
