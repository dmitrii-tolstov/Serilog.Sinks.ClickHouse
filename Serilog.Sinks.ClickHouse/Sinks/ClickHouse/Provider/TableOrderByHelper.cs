using System;
using System.Collections.Generic;
using System.Linq;
using Serilog.Debugging;

namespace Serilog.Sinks.ClickHouse.Provider
{
    /// <summary>
    /// Generation ORDER BY script
    /// </summary>
    class TableOrderByHelper
    {
        protected IEnumerable<string> fieldNameList;
        protected int fieldCount;

        protected TableOrderByHelper(IEnumerable<string> orderByFieldList)
        {
            fieldNameList = orderByFieldList;
            fieldCount = fieldNameList.Count();
        }

        protected string GetScriptInternal()
        {
            string script = "ORDER BY ";

            // Default ORDER BY
            if (fieldNameList is null)
            {
                return script + "timestamp";
            }

            // Without ORDER BY
            if (fieldCount == 0
             || (fieldCount == 1 && fieldNameList.First().Equals("tuple()",StringComparison.OrdinalIgnoreCase)))
            {
                return script += "tuple()";
            }

            if (ValidateFieldNameList())
            {
                script += $"({string.Join(",", fieldNameList)})";
            }
            else
            {
                throw new ArgumentException("Wrong section: OrderBy");
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
                SelfLog.WriteLine($"OrderBy: Wrong field name: {fieldName}");
                validateResult = false;
            }

            return validateResult;
        }

        /// <summary>
        /// Generate ORDER BY script
        /// </summary>
        /// <param name="orderByFieldList">Field collection</param>
        /// <returns>script</returns>
        public static string GetScript(IEnumerable<string> orderByFieldList)
        {
            return new TableOrderByHelper(orderByFieldList).GetScriptInternal();
        }
    }
}