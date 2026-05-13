using System;
using System.Collections.Generic;
using System.Linq;
using Serilog.Debugging;

namespace Serilog.Sinks.ClickHouse.Ado.Provider
{
    /// <summary>
    /// Generation PARTITION BY script
    /// </summary>
    class TablePartitionByHelper
    {
        protected IEnumerable<string> fieldNameList;
        protected int fieldCount;

        protected TablePartitionByHelper(IEnumerable<string> partitionByFieldList)
        {
            fieldNameList = partitionByFieldList;
            fieldCount = fieldNameList.Count();
        }

        protected string GetScriptInternal()
        {
            string script = "PARTITION BY ";

            // Default PARTITION BY
            if (fieldNameList is null
             || fieldCount == 0)
            {
                return "";
            }

            if (ValidateFieldNameList())
            {
                script += $"({string.Join(",", fieldNameList)})";
            }
            else
            {
                throw new ArgumentException("Wrong section: PartitionBy");
            }

            return script;
        }

        protected bool ValidateFieldNameList()
        {
            bool validateResult = true;

            foreach (var fieldName in fieldNameList)
            {
                validateResult = ValidateFieldName(fieldName) && validateResult;
            }

            return validateResult;
        }

        protected static bool ValidateFieldName(string fieldName)
        {
            bool validateResult = true;

            if (fieldName.Trim() == "")
            {
                SelfLog.WriteLine($"PartitionBy: Wrong field name: {fieldName}");
                validateResult = false;
            }

            return validateResult;
        }

        /// <summary>
        /// Generate PARTITION BY script
        /// </summary>
        /// <param name="partitionByFieldList">Field collection</param>
        /// <returns>script</returns>
        public static string GetScript(IEnumerable<string> partitionByFieldList)
        {
            return new TablePartitionByHelper(partitionByFieldList).GetScriptInternal();
        }
    }
}