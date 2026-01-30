using osu.Game.Beatmaps.Formats;
using osu.Game.IO;
using osu.Game.Rulesets.Mania;
using osu.Game.Rulesets.Mania.Beatmaps;

namespace UnbeatableConverter.Core.Beatmap;

public class BeatmapConverter
{
    /// Decode the beatmap from the provided stream and convert it to a ManiaBeatmap.
    public ManiaBeatmap DecodeBeatmap(Stream beatmapStream)
    {
        using var osuReader = new LineBufferedReader(beatmapStream);

        var decoder = new LegacyBeatmapDecoder();
        var beatmap = decoder.Decode(osuReader);

        // Fix key, change mode
        beatmap.Difficulty.CircleSize = 5;
        beatmap.BeatmapInfo.Ruleset.OnlineID = 3; // osu!mania


        var maniaRuleset = new ManiaRuleset();
        var converter = new ManiaBeatmapConverter(beatmap, maniaRuleset)
        {
            TargetColumns = 2, // 2-key mod conversion
            Dual = false
        };

        var convertedBeatmap = converter.Convert() as ManiaBeatmap ??
                               throw new InvalidOperationException("Conversion to ManiaBeatmap failed.");

        foreach (var hitObject in convertedBeatmap.HitObjects)
        {
            hitObject.Samples.Clear();

            hitObject.Column += 2; // Shift to center columns 3 and 4
        }

        return convertedBeatmap;
    }

    /// Encode the ManiaBeatmap for output.
    /// Returns a Stream containing the encoded beatmap data. (Position set to 0)
    /// The caller is responsible for disposing the returned Stream.
    public Stream EncodeBeatmap(ManiaBeatmap beatmap)
    {
        var outputStream = new MemoryStream();
        var writer = new StreamWriter(outputStream);
        writer.NewLine = "\r\n";

        var encoder = new BeatmapEncoder(beatmap);
        encoder.EncodeBeatmap(writer);
        writer.Flush();

        outputStream.Position = 0;
        return outputStream;
    }
}