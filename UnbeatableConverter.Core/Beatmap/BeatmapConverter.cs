using osu.Game.Beatmaps.Formats;
using osu.Game.IO;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Mania;
using osu.Game.Rulesets.Mania.Beatmaps;
using osu.Game.Rulesets.Mania.Objects;
using osu.Game.Rulesets.Objects;

namespace UnbeatableConverter.Core.Beatmap;

public class BeatmapConverter
{
    /// Decode the beatmap from the provided stream and convert it to an UNBEATABLE-ready ManiaBeatmap.
    public ManiaBeatmap DecodeBeatmap(Stream beatmapStream)
    {
        using var osuReader = new LineBufferedReader(beatmapStream);

        var decoder = new LegacyBeatmapDecoder();
        var beatmap = decoder.Decode(osuReader);

        var wasManiaBefore = beatmap.BeatmapInfo.Ruleset.OnlineID == 3;

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

        // Mania beatmaps tend to have overlapping notes when compressed to
        // 2 keys, so we need to remove duplicates here.
        if (wasManiaBefore)
        {
            var duplicateRemover = new DuplicateRemover();
            duplicateRemover.Convert(convertedBeatmap);
        }

        foreach (var hitObject in convertedBeatmap.HitObjects)
        {
            hitObject.Samples.Clear();

            // NOTE: Columns start at 0, so Column 2 and 3 are the center
            hitObject.Column += 2; // Shift to center columns
        }


        // TODO: Separate class for this?

        // TODO: Add spikes and double notes based on original beatmap


        // TODO: Add flips during kiai time
        var controlPoints = convertedBeatmap.ControlPointInfo;
        var kiaiPoints = controlPoints.EffectPoints;
        for (var i = 0; i < kiaiPoints.Count; i++)
        {
            var kiaiPoint = kiaiPoints[i];
            if (kiaiPoint.KiaiMode)
            {
                // Get nearest beat
                var time = kiaiPoint.Time;

                var timingPoint = controlPoints.TimingPointAt(time);

                // Place zoom start
                var zoomStart = controlPoints.GetClosestSnappedTime(time) - timingPoint.BeatLength;

                var zoomStartObject = new Note()
                {
                    StartTime = zoomStart,
                };
                zoomStartObject.ApplyModifier(NoteModifier.Zoom);
                zoomStartObject.Column = 4; // 5th Column for camera controls

                // Place zoom end
                var kiaiEndTime = (i + 1 < kiaiPoints.Count)
                    ? kiaiPoints[i + 1].Time
                    : beatmap.HitObjects.Last().GetEndTime();

                var zoomEnd = controlPoints.GetClosestSnappedTime(kiaiEndTime) + timingPoint.BeatLength;
                var zoomEndObject = new Note()
                {
                    StartTime = zoomEnd,
                };
                zoomEndObject.ApplyModifier(NoteModifier.Zoom);
                zoomEndObject.Column = 4;


                // Add flips every 4 beats during kiai
                for (double t = time; t < kiaiEndTime; t += timingPoint.BeatLength * 4)
                {
                    var flipObject = new Note()
                    {
                        StartTime = t,
                    };

                    flipObject.Column = 4;

                    convertedBeatmap.HitObjects.Add(flipObject);
                }

                convertedBeatmap.HitObjects.Add(zoomStartObject);
                convertedBeatmap.HitObjects.Add(zoomEndObject);
            }
        }

        convertedBeatmap.HitObjects.Sort((a, b) => a.StartTime.CompareTo(b.StartTime));

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