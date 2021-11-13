using System;
using System.Collections.Generic;
using System.Linq;

namespace SQLiteMapper
{
    public static class Utils
    {
        public static Dictionary<string, string?> GetSqLiteTypeDict(IEnumerable<Dictionary<string, object>> valueList,
            Dictionary<string, string>? dictionary)
        {
            var typeDict = dictionary is null
                ? new Dictionary<string, string?>()
                : new Dictionary<string, string?>(dictionary!);
            foreach (var row in valueList) {
                foreach (var (key, value) in row) {
                    if (!typeDict.ContainsKey(key) || typeDict[key] is null) {
                        typeDict[key] = GetSqLiteType(value);
                    }
                }

                if (typeDict.Values.All(v => v is not null)) {
                    return typeDict;
                }
            }

            // set all remaining nulls to type "TEXT"
            foreach (var key in typeDict.Keys) {
                typeDict[key] = typeDict[key] is null ? "TEXT" : typeDict[key];
            }

            return typeDict;
        }

        /// <summary>
        /// Tries to map a value to a sqlite value
        /// </summary>
        /// <param name="value"></param>
        /// <returns>sqlite value</returns>
        public static object ToSqliteValue(object? value)
        {
            if (value is null) return "null";
            return value switch {
                bool l => l ? 1 : 0,
                long l => value,
                double d => value,
                DateTime dt => $"'{value}'",
                string str => $"'{value}'",
                _ => throw new Exception($"ToSqliteValue hit default with object of type {value.GetType()}")
            };
        }

        /// <summary>
        /// Tries to map a value to a sqlite type
        /// </summary>
        /// <param name="value"></param>
        /// <returns>sqlite type</returns>
        private static string GetSqLiteType(object value)
        {
            if (value is null) return null;
            return value switch {
                bool l => "INTEGER",
                long l => "INTEGER",
                double d => "REAL",
                DateTime dt => "TEXT",
                string str => "TEXT",
                _ => throw new Exception($"GetSqLiteType hit default with object of type {value.GetType()}")
            };
        }
    }
}