﻿using System;
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
            
            // var sortedDict = input.data.ToImmutableSortedDictionary(StringComparer.InvariantCultureIgnoreCase);
            // var createStatementsBuilder = new StringBuilder();
            // var typeDictList = sortedDict.Values.Select(GetSqLiteTypeDict);
            // foreach (Dictionary<string,string> dictionary in typeDictList) {
            //     createStatementsBuilder.Append($"CREATE TABLE {tableName}\n(\n\t");
            //     var typeDict = GetSqLiteTypeDict(valueList);
            //     createStatementsBuilder.AppendJoin(",\n\t", typeDict.Select(pair => $"{pair.Key} {pair.Value}"));
            //     createStatementsBuilder.Append("\n);\n");
            // }
            
            
            
            
            var createTablesStatement = GenerateCreateTablesStatement(input);
            var createInsertsStatement = GenerateCreateInsertsStatement(input);
            return createTablesStatement + createInsertsStatement;
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

        private static Dictionary<string, string> GetSqLiteTypeDict(IEnumerable<Dictionary<string, object>> valueList)
        {
            var typeDict = new Dictionary<string, string>();
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

            var errorString = $"Could not guess the types of following columns: [{String.Join(", ", typeDict.Where(pair => pair.Value is null).Select(p => p.Key))}], probably because their values are all null. You need to declase types of these columns explicitly (NOT IMPLEMENTED)";
            throw new ArgumentNullException(errorString);
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