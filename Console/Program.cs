using System.IO;
using CommandLine;

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

            // for each input, load file
            foreach (string filename in args) {
                using var o = File.OpenText(filename);
                var fileText = o.ReadToEnd();
                System.Console.WriteLine(fileText);
            }
            // for each file, call Generate input
            // for each file, save to FILENAME_insert.sql
        }

        private static string GetHelpText()
        {
            return @"
Specify one or more json files for which (hopefully) corresponding sqlite create and insert statements will be produced.
Bottom level element must be array of objects.
The name of table created will be lowered file name (eg. Person.json -> person).
Output file(s) will be called FILENAME-insert.sql (eg. Person.json -> Person-insert.sql).
After this, create a sqlite database with sqlite3 cli like so: sqlite3 somename.db < Person-insert.sql
                ";
        }
    }
}