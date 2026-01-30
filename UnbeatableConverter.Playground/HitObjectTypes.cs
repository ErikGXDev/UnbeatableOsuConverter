using System.IO.Compression;
using osu.Game.Beatmaps.Formats;
using osu.Game.IO;
using osu.Game.Rulesets.Mania.Objects;

namespace UnbeatableConverter.Playground;

public class HitObjectTypes
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

        using var osuReader = new LineBufferedReader(entryStream);


        var decoder = new LegacyBeatmapDecoder();
        var beatmap = decoder.Decode(osuReader);

        Console.WriteLine("Mode: " + beatmap.BeatmapInfo.Ruleset.OnlineID);

        // Fix key, change mode
        beatmap.Difficulty.CircleSize = 5;
        beatmap.BeatmapInfo.Ruleset.OnlineID = 3; // osu!mania


        foreach (var obj in beatmap.HitObjects)
        {
            Console.WriteLine($"HitObject Type: {obj.GetType().Name}");
            if (obj is ManiaHitObject maniaHitObject)
            {
                Console.WriteLine($"  Column: {maniaHitObject.Column}");
            }
        }
    }
}