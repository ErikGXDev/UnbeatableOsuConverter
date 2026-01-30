using UnbeatableConverter.Core;

namespace UnbeatableConverter.CLI;

public class DebugProgram
{
    public static void MainRun(string[] args)
    {
        Console.WriteLine("Debug Program Running...");

        var inputPath = "C:\\Users\\Anwender\\Downloads\\1861042 Hinkik - Explorers.osz";

        var converter = new OszExporter(inputPath);

        var outputPath = converter.ExportFull();

        Console.WriteLine($"Converted file saved to: {outputPath}");
    }
}