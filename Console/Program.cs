using System.Collections.Generic;
using System.IO;
using CommandLine;
using Newtonsoft.Json;
using SQLiteMapper;

namespace Console
{
    class Program
    {
        public class Options
        {
            [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
            public bool Verbose { get; set; }
        }

        static void Main(string[] args)
        {
            if (args.Length == 0) {
                System.Console.WriteLine(GetHelpText());
                return;
            }

            // for each input, load file -> call sqlitemapper -> write to stdout
            foreach (string filename in args) {
                using var o = File.OpenText(filename);
                var fileText = o.ReadToEnd();
                SqLiteMapperInput mapperInput = GetSqLiteMapperInput(filename, fileText);
                var sqlOutput = SqLiteMapper.GenerateTableAndInsertStatements(mapperInput);
                System.Console.WriteLine(sqlOutput);
            }
        }

        private static SqLiteMapperInput GetSqLiteMapperInput(string filename, string fileText)
        {
            return new SqLiteMapperInput {
                data = new Dictionary<string, IEnumerable<Dictionary<string, object>>> {
                    {
                        filename.Replace(".json", "").ToLower(),
                        JsonConvert.DeserializeObject<IEnumerable<Dictionary<string, object>>>(fileText)
                    }
                }
            };
        }

        private static string GetHelpText()
        {
            return @"
Specify one or more json files for which (hopefully) corresponding sqlite create and insert statements will be produced.
Bottom level element must be array of objects.
The name of table created will be lowered file name (eg. Person.json -> person).
Output will be written to stdout. Recommend piping to sql file (eg Person-insert.sql)
After this, create a sqlite database with sqlite3 cli like so: sqlite3 somename.db < Person-insert.sql
                ";
        }
    }
}