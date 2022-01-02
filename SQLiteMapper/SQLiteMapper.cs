﻿using System;
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
        /// <summary>
        /// Generate table and insert statements from data. Tries to infer type if not specified in input.schema.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GenerateTableAndInsertStatements(SqLiteMapperInput input)
        {
            RemoveInvalidCharsInPropNames(input);
            ValidationHelper.ValidateSchemaIfPresent(input);

            var tableBuilder = new StringBuilder();
            var insertBuilder = new StringBuilder();
            var sortedDict = input.data.ToImmutableSortedDictionary(StringComparer.InvariantCultureIgnoreCase);

            foreach (var (tableName, valueList) in sortedDict) {
                var schema = input.schemas?[tableName];
                var typeDict = Utils.GetSqLiteTypeDict(valueList, schema);

                tableBuilder.Append($"CREATE TABLE {tableName}\n(\n\t");
                tableBuilder.AppendJoin(",\n\t", typeDict.Select(pair => $"{pair.Key} {pair.Value}"));
                tableBuilder.Append("\n);\n");

                if (valueList.Any()) {
                    insertBuilder.Append($"INSERT INTO {tableName} ({String.Join(", ", typeDict.Keys)})\nVALUES ");
                    insertBuilder.AppendJoin(",\n\t",
                        valueList.Select(row => $"({String.Join(", ", row.Values.Select(Utils.ToSqliteValue))})"));
                    insertBuilder.Append(";\n\n");
                }
            }

            return tableBuilder + insertBuilder.ToString();
        }

        /// <summary>
        /// Removes invalid chars from sqlite table property names, such as '@'
        /// This method makes changes to input
        /// </summary>
        /// <param name="input"></param>
        private static void RemoveInvalidCharsInPropNames(SqLiteMapperInput input)
        {
            foreach (var table in input.data) {
                foreach (Dictionary<string, object> objDict in table.Value) {
                    var keysWithInvalidChars = objDict.Keys.Where(k => k.Contains("@")).ToImmutableArray();
                    foreach (string invalidKey in keysWithInvalidChars) {
                        objDict[invalidKey.Replace("@", "")] = objDict[invalidKey];
                        objDict.Remove(invalidKey);
                    }
                }
            }
        }


        /// <summary>
        /// Executes sqlite query in on data
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
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
                var queryResult = connection.Query(input.query);
                return JsonConvert.SerializeObject(queryResult);
            }
        }
    }
}