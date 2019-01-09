using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            string table = "";
            string lineAccumulator = "";
            List<string> output = new List<string>();
            Regex createTable = new Regex(@"[ ]*CREATE TABLE IF NOT EXISTS `[_a-zA-Z0-9]*`\.`([_a-zA-Z0-9]*)` \(");
            Regex contstraint = new Regex(@"[ ]*CONSTRAINT `[_a-zA-Z0-9]*`");
            Regex finalForeignKey = new Regex(@"CONSTRAINT `[_a-zA-Z0-9]*`[ ]*FOREIGN KEY \(`[_a-zA-Z0-9]*`\)[ ]*REFERENCES `[_a-zA-Z0-9]*`\.`[_a-zA-Z0-9]*` \(`[_a-zA-Z0-9]*`\)([ ]*ON DELETE ((CASCADE)|(SET NULL)))?([ ]*ON UPDATE ((CASCADE)|(SET NULL)))?(,)?");
            Regex index = new Regex(@"(UNIQUE )?INDEX `[_a-zA-Z0-9]*` \((`[_a-zA-Z0-9]*`(\([\d]*\))?( ASC| DESC)?(, )?)*\)");

            const string inputFilePath = @"<ENTER INPUT FILE PATH HERE>";
            const string dbName = "<ENTER DB NAME HERE>";

            foreach (var line in System.IO.File.ReadAllLines(inputFilePath))
            {
                table = GetTableNameMatch(table, createTable, line);
                if (index.IsMatch(line))
                {
                    output.Add($"ALTER TABLE `{dbName}`.`{table}` ADD {index.Match(line).Value};");
                }
            }
            table = "";

            output.Add("-------------------FKs from here------------------");

            foreach (var line in System.IO.File.ReadAllLines(inputFilePath))
            {
                table = GetTableNameMatch(table, createTable, line);
                if (finalForeignKey.IsMatch(lineAccumulator)
                    && finalForeignKey.Match(lineAccumulator + line).Length + finalForeignKey.Match(lineAccumulator + line).Index + 1 < lineAccumulator.Length + line.Length)
                {
                    if (lineAccumulator.EndsWith(")") || lineAccumulator.EndsWith(","))
                    {
                        lineAccumulator = lineAccumulator.TrimEnd(')', ',');
                    }
                    if (lineAccumulator.EndsWith("`"))
                    {
                        lineAccumulator += ')';
                    }
                    output.Add($"ALTER TABLE `{dbName}`.`{table}` ADD {lineAccumulator};");
                    lineAccumulator = "";
                }
                if (contstraint.IsMatch(line))
                {
                    lineAccumulator = line;
                }
                else
                {
                    lineAccumulator += line;
                }
            }

            System.IO.File.WriteAllLines(@"<ENTER OUTPUT FILE PATH HERE>", output.ToArray());
        }

        private static string GetTableNameMatch(string table, Regex createTable, string line)
        {
            if (createTable.IsMatch(line))
            {
                table = createTable.Match(line).Groups[1].Value;
            }
            return table;
        }
    }
}
