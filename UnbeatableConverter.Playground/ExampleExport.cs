using UnbeatableConverter.Core;

namespace UnbeatableConverter.Playground;

public class ExampleExport
{
    public static void Run()
    {
        Console.WriteLine("Debug Program Running...");

        var inputPath = "C:\\Users\\Anwender\\Downloads\\1861042 Hinkik - Explorers.osz";

        var converter = new OszExporter(inputPath);

        var outputPath = converter.ExportFull();

        Console.WriteLine($"Converted file saved to: {outputPath}");
    }
}