using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using Dapper;
using Newtonsoft.Json.Linq;

namespace SQLiteMapper
{
    public static class SqLiteMapper
    {
        public static string Export(string input)
        {
            var parsedInput = JsonConvert.DeserializeObject<SqLiteMapperInput>(input);
            var createTablesStatement = GenerateCreateTablesStatement(parsedInput);
            var createInsertsStatement = GenerateCreateInsertsStatement(parsedInput);
            return createTablesStatement + createInsertsStatement;
        }

        private static string GenerateCreateInsertsStatement(SqLiteMapperInput parsedInput)
        {
            var builder = new StringBuilder();
            var sortedDict = parsedInput.data.ToImmutableSortedDictionary(StringComparer.InvariantCultureIgnoreCase);
            foreach (var (tableName, valueList) in sortedDict) {
                var typeDict = GetSqLiteTypeDict(valueList);
                builder.Append($"INSERT INTO {tableName} ({String.Join(", ", typeDict.Keys)})\nVALUES ");
                builder.AppendJoin(",\n\t", valueList.Select(row => $"({String.Join(", ", row.Values.Select(ToSqliteValue))})"));
                builder.Append(";\n\n");
            }

            return builder.ToString();
        }


        private static string GenerateCreateTablesStatement(SqLiteMapperInput parsedInput)
        {
            var builder = new StringBuilder();
            var sortedDict = parsedInput.data.ToImmutableSortedDictionary(StringComparer.InvariantCultureIgnoreCase);
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
                // JObject jObject => GetSqLiteType(jObject),
                // JArray jArray => GetSqLiteType(jArray),
                // JValue jValue => GetSqLiteType(jValue),
                bool l => "INTEGER",
                long l => "INTEGER", // gets sent to double for some reason, which works
                double d => "REAL",
                DateTime dt => "TEXT",
                string str => "TEXT",
                _ => throw new Exception($"GetSqLiteType hit default with object of type {value.GetType()}")
            };
        }

        // new SqliteConnection("Data Source=C:\\Users\\Marph\\source\\SQLiteMapper\\SQLiteMapper\\testdb")) {
        public static string Execute(string input)
        {
            using (var connection = new SqliteConnection("Data Source=:memory:")) {
                connection.Open();

                // insert import statement
                using (var transaction = connection.BeginTransaction()) {
                }

                var insertCommand = connection.CreateCommand();
                insertCommand.CommandText =
                    @"
                    CREATE TABLE users
                    (
                        name TEXT,
                        age  INTEGER
                    );
                    INSERT INTO users (name, age)
                    VALUES ('Martin', 42),
                           ('Jørgen', 2),
                           ('Ali', 999),
                           ('Jørgen', null);
                    ";
                insertCommand.ExecuteNonQuery();

                var la = connection.Query("select * from users");
                var o = JsonConvert.SerializeObject(la);
                Console.WriteLine(o);
            }

            return "Should not happen";
        }

        public static void Init()
        {
            using (var connection =
                new SqliteConnection("Data Source=C:\\Users\\Marph\\source\\SQLiteMapper\\SQLiteMapper\\testdb")) {
                // using (var connection = new SqliteConnection("Data Source=:memory:")) {
                connection.Open();
                Console.WriteLine(connection.DataSource);
                using (var transaction = connection.BeginTransaction()) {
                    var command = connection.CreateCommand();
                    command.CommandText =
                        @"
                            INSERT INTO data
                            VALUES ($value)
                        ";

                    var parameter = command.CreateParameter();
                    parameter.ParameterName = "$value";
                    command.Parameters.Add(parameter);

                    // Insert a lot of data
                    var random = new Random();
                    for (var i = 0; i < 150_000; i++) {
                        parameter.Value = random.Next();
                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
            }
        }
    }
}