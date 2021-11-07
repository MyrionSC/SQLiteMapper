using System;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using Dapper;

namespace SQLiteMapper
{
    public static class SqLiteMapper
    {
        // Remember to use transaction
        public static string Export(string input)
        {
            var parsedInput = JsonConvert.DeserializeObject<SqLiteMapperInput>(input);


            using (var connection =
                new SqliteConnection("Data Source=C:\\Users\\Marph\\source\\SQLiteMapper\\SQLiteMapper\\testdb")) {
                connection.Open();

                using (var transaction = connection.BeginTransaction()) {
                }


                // read json into db
                // var json = JsonConvert.DeserializeObject(input);
                // Console.WriteLine(json);

                // generate export statement


                // var command = connection.CreateCommand();
                // command.CommandText =
                //     @"
                //         SELECT first_name
                //         FROM contacts
                //         WHERE contact_id = $id
                //     ";
                // command.Parameters.AddWithValue("$id", "123");
                //
                // using (var reader = command.ExecuteReader()) {
                //     while (reader.Read()) {
                //         var name = reader.GetString(0);
                //
                //         return $"{name}";
                //     }
                // }
            }

            return "Should not happen";
        }

        // new SqliteConnection("Data Source=C:\\Users\\Marph\\source\\SQLiteMapper\\SQLiteMapper\\testdb")) {
        public static string Execute(string input)
        {
            using (var connection = new SqliteConnection("Data Source=:memory:")) {
                connection.Open();

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

                var la = connection.Query("select name from users");
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