using System.Diagnostics;
using UnbeatableConverter.Core;

namespace UnbeatableConverter.CLI;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("Usage: UnbeatableConverter.CLI <input.osz>");
            return;
        }

        var inputPath = args[0];

        if (!File.Exists(inputPath))
        {
            Console.WriteLine($"Input file does not exist: {inputPath}");
            return;
        }

        var converter = new OszExporter(inputPath);

        var outputPath = converter.ExportFull();

        Console.WriteLine($"Converted file saved to: {outputPath}");
        Console.WriteLine("Press Enter to exit...");
        Console.ReadLine();
    }
}