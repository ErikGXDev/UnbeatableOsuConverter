using System.Diagnostics;
using UnbeatableConverter.Core;

namespace UnbeatableConverter.CLI;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        // TODO: Remove when CLI is finished
        TryDebugProgram(args);

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

    [Conditional("DEBUG")]
    static void TryDebugProgram(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("No arguments provided, running debug for now.");
            DebugProgram.MainRun(args);
        }
    }
}