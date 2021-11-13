using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using Dapper;
using SQLiteMapper.Exceptions;

namespace SQLiteMapper
{
    public static class SqLiteMapper
    {
        public static string GenerateTableAndInsertStatements(SqLiteMapperInput input)
        {
            ValidateSchemaIfPresent(input);
            
            var tableBuilder = new StringBuilder();
            var insertBuilder = new StringBuilder();
            var sortedDict = input.data.ToImmutableSortedDictionary(StringComparer.InvariantCultureIgnoreCase);
            
            foreach (var (tableName, valueList) in sortedDict) {
                var schema = input.schemas?[tableName];
                var typeDict = GetSqLiteTypeDict(valueList, schema);
                
                tableBuilder.Append($"CREATE TABLE {tableName}\n(\n\t");
                tableBuilder.AppendJoin(",\n\t", typeDict.Select(pair => $"{pair.Key} {pair.Value}"));
                tableBuilder.Append("\n);\n");
                
                insertBuilder.Append($"INSERT INTO {tableName} ({String.Join(", ", typeDict.Keys)})\nVALUES ");
                insertBuilder.AppendJoin(",\n\t",
                    valueList.Select(row => $"({String.Join(", ", row.Values.Select(ToSqliteValue))})"));
                insertBuilder.Append(";\n\n");
            }
            
            return tableBuilder + insertBuilder.ToString();
        }

        private static void ValidateSchemaIfPresent(SqLiteMapperInput input)
        {
            if (input.schemas is null) return;
            
            // validate that schemas values are valid sqlite types
            var validTypeList = new[] { "TEXT", "INTEGER", "REAL", "BLOB" };
            foreach (var schema in input.schemas) {
                if (schema.Value is null) throw new ArgumentNullException(nameof(schema));
                foreach (var schemaKeyValue in schema.Value) {
                    if (!validTypeList.Contains(schemaKeyValue.Value)) {
                        throw new SchemaBadValueException($"declared value in schema keyvalue pair ({schemaKeyValue.Key}, {schemaKeyValue.Value}) is not valid sqlite type (which are: {JsonConvert.SerializeObject(validTypeList)})");
                    }
                }
            }
            
            // validate that schema keys correspond to data
            foreach (var key in input.schemas.Keys) {
                if (!input.data.ContainsKey(key)) {
                    throw new SchemaNoCorrespondingDataException($"schema key {key} does not have a corresponding data key");
                }
            }
        }

        private static Dictionary<string, string?> GetSqLiteTypeDict(IEnumerable<Dictionary<string, object>> valueList,
            Dictionary<string, string>? dictionary)
        {
            var typeDict = dictionary is null ? 
                new Dictionary<string, string?>() :
                new Dictionary<string, string?>(dictionary!);
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
        private static object ToSqliteValue(object? value)
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