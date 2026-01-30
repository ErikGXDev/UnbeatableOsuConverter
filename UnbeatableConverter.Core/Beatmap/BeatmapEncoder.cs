using osu.Game.Beatmaps;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Beatmaps.Formats;

namespace UnbeatableConverter.Core.Beatmap;

public class BeatmapEncoder : LegacyBeatmapEncoder
{
    public BeatmapEncoder(IBeatmap beatmap) : base(beatmap, null)
    {
        foreach (TimingControlPoint tcp in beatmap.ControlPointInfo.TimingPoints.ToList())
        {
            //tcp.Time = Math.Floor(tcp.Time);
        }
    }

    /// Encode the beatmap to the provided TextWriter.
    public void EncodeBeatmap(TextWriter writer)
    {
        using var tempWriter = new StringWriter();
        tempWriter.NewLine = writer.NewLine;

        Encode(tempWriter);

        string output = tempWriter.ToString();

        string[] splitLines = output.Split(writer.NewLine);
        foreach (string line in splitLines.ToList())
        {
            if (line.StartsWith("512,"))
            {
                string newLine = line.Replace("512,", "511,");
                writer.WriteLine(newLine);
            }
            else
            {
                writer.WriteLine(line);
            }
        }
    }
}