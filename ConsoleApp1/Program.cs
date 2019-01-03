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
            Regex final = new Regex(@"CONSTRAINT `[_a-zA-Z0-9]*`[ ]*FOREIGN KEY \(`[_a-zA-Z0-9]*`\)[ ]*REFERENCES `[_a-zA-Z0-9]*`\.`[_a-zA-Z0-9]*` \(`[_a-zA-Z0-9]*`\)([ ]*ON DELETE ((CASCADE)|(SET NULL)))?([ ]*ON UPDATE ((CASCADE)|(SET NULL)))?(,)?");

            foreach (var line in System.IO.File.ReadAllLines(@"<ENTER INPUT FILE PATH HERE>"))
            {
                if (createTable.IsMatch(line))
                {
                    table = createTable.Match(line).Groups[1].Value;
                }
                if (final.IsMatch(lineAccumulator)
                    && final.Match(lineAccumulator + line).Length + final.Match(lineAccumulator + line).Index + 1 < lineAccumulator.Length + line.Length )
                {
                    if (lineAccumulator.EndsWith(")") || lineAccumulator.EndsWith(","))
                    {
                        lineAccumulator = lineAccumulator.TrimEnd(')', ',');
                    }
                    if (lineAccumulator.EndsWith("`"))
                    {
                        lineAccumulator += ')';
                    }
                    output.Add($"ALTER TABLE `<ENTER DBNAME HERE>`.`{table}` ADD {lineAccumulator};");
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
    }
}
