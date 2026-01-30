using System.IO.Compression;
using osu.Game.Rulesets.Objects;
using UnbeatableConverter.Core;
using UnbeatableConverter.Core.Beatmap;

namespace UnbeatableConverter.Playground;

public class KiaiDebug
{
    public static void Run()
    {
        Console.WriteLine("Debug Program Running...");

        var inputPath = @"C:\Users\Anwender\Downloads\2425769 Good Kid - No Time to Explain (Cut Ver.).osz";
        var entryFile = "Good Kid - No Time to Explain (Cut Ver.) (keksikosu) [Normal].osu";

        using var zipStream = ZipFile.OpenRead(inputPath);

        var zipEntry = zipStream.GetEntry(entryFile);

        if (zipEntry == null)
        {
            Console.WriteLine("Entry not found");
            return;
        }

        using var entryStream = zipEntry.Open();

        var converter = new BeatmapConverter();
        var beatmap = converter.DecodeBeatmap(entryStream);


        var effectPoints = beatmap.ControlPointInfo.EffectPoints;

        Console.WriteLine($"Total EffectPoints: {effectPoints.Count}");

        for (int i = 0; i < effectPoints.Count; i++)
        {
            Console.WriteLine($"{i + 1}. Kiai: {effectPoints[i].KiaiMode}");
            if (effectPoints[i].KiaiMode)
            {
                double kiaiStartTime = effectPoints[i].Time;
                double kiaiEndTime = (i + 1 < effectPoints.Count)
                    ? effectPoints[i + 1].Time
                    : beatmap.HitObjects.Last().GetEndTime();

                Console.WriteLine($"Kiai from {kiaiStartTime} to {kiaiEndTime}");
            }
        }

        Console.WriteLine("Done");
    }
}