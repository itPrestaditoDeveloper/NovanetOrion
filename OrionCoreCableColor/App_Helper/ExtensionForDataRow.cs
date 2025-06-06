using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;


namespace OrionCoreCableColor.App_Helper
{
    public static class ExtensionForDataRow
    {
        public static T? GetDataRowValue<T>(this DataRow row, string columnName) where T : struct
        {
            if (row.IsNull(columnName))
                return null;

            //return row[columnName] as T?;
            var result = new T?();
            try
            {
                if (!string.IsNullOrEmpty(row[columnName].ToString()) && row[columnName].ToString().Trim().Length > 0)
                {
                    TypeConverter conv = TypeDescriptor.GetConverter(typeof(T));
                    var convertFrom = conv.ConvertFrom(row[columnName].ToString());
                    if (convertFrom != null)
                        result = (T)convertFrom;
                }
            }
            catch { }
            return result;

        }

        public static string GetTextFromRow(this DataRow row, string columnName)
        {
            if (row.IsNull(columnName))
                return string.Empty;

            return row[columnName] as string ?? string.Empty;
        }

    }
}