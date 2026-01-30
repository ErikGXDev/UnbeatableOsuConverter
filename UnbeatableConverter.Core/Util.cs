namespace UnbeatableConverter.Core;

public class Util
{
    public static string[] beatmapExtensions = new string[] { ".txt", ".osu" };

    public static string[] zipExtensions = new string[] { ".zip", ".osz" };

    // Check if the file is a beatmap file
    public static bool IsBeatmapFile(string path)
    {
        var ext = Path.GetExtension(path).ToLower();
        return beatmapExtensions.Contains(ext);
    }

    // Check if the file is a zip file
    // it will most likely contain multiple beatmaps
    public static bool IsZipFile(string path)
    {
        var ext = Path.GetExtension(path).ToLower();
        return zipExtensions.Contains(ext);
    }
}