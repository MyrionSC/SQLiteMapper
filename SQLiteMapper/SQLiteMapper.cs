using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using Dapper;

namespace SQLiteMapper
{
    public static class SqLiteMapper
    {
        public static string GenerateTableAndInsertStatements(SqLiteMapperInput input)
        {
            var createTablesStatement = GenerateCreateTablesStatement(input);
            var createInsertsStatement = GenerateCreateInsertsStatement(input);
            return createTablesStatement + createInsertsStatement;
        }

        private static string GenerateCreateInsertsStatement(SqLiteMapperInput input)
        {
            var builder = new StringBuilder();
            var sortedDict = input.data.ToImmutableSortedDictionary(StringComparer.InvariantCultureIgnoreCase);
            foreach (var (tableName, valueList) in sortedDict) {
                var typeDict = GetSqLiteTypeDict(valueList);
                builder.Append($"INSERT INTO {tableName} ({String.Join(", ", typeDict.Keys)})\nVALUES ");
                builder.AppendJoin(",\n\t",
                    valueList.Select(row => $"({String.Join(", ", row.Values.Select(ToSqliteValue))})"));
                builder.Append(";\n\n");
            }

            return builder.ToString();
        }

        private static string GenerateCreateTablesStatement(SqLiteMapperInput input)
        {
            var builder = new StringBuilder();
            var sortedDict = input.data.ToImmutableSortedDictionary(StringComparer.InvariantCultureIgnoreCase);
            foreach (var (tableName, valueList) in sortedDict) {
                builder.Append($"CREATE TABLE {tableName}\n(\n\t");
                var typeDict = GetSqLiteTypeDict(valueList);
                builder.AppendJoin(",\n\t", typeDict.Select(pair => $"{pair.Key} {pair.Value}"));
                builder.Append("\n);\n");
            }

            return builder.ToString();
        }


        private static Dictionary<string, string> GetSqLiteTypeDict(IEnumerable<Dictionary<string, object>> valueList)
        {
            var typeDict = new Dictionary<string, string>();
            foreach (var row in valueList) {
                foreach (var (key, value) in row) {
                    if (!typeDict.ContainsKey(key) || typeDict[key] is null) {
                        // TODO: finish early somehow
                        typeDict[key] = GetSqLiteType(value);
                    }
                }
            }

            return typeDict;
        }

        /// <summary>
        /// Tries to map a value to a sqlite value
        /// </summary>
        /// <param name="value"></param>
        /// <returns>sqlite value</returns>
        private static object ToSqliteValue(object value)
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
                long l => "INTEGER", // gets sent to double for some reason, which works
                double d => "REAL",
                DateTime dt => "TEXT",
                string str => "TEXT",
                _ => throw new Exception($"GetSqLiteType hit default with object of type {value.GetType()}")
            };
        }

        public static string ExecuteQuery(SqLiteMapperInput input)
        {
            var statements = GenerateTableAndInsertStatements(input);
            using (var connection = new SqliteConnection("Data Source=:memory:")) {
                connection.Open();

                // we wrap in transaction because the internet tells me it is faster. Otherwise each stmt gets transaction each. TODO: test performance.
                using (var transaction = connection.BeginTransaction()) {
                    var insertCommand = connection.CreateCommand();
                    insertCommand.CommandText = statements;
                    insertCommand.ExecuteNonQuery();
                    transaction.Commit();
                }

                // By the power of Dapper, execute the query!
                var la = connection.Query(input.query);
                return JsonConvert.SerializeObject(la);
            }
        }
    }
}