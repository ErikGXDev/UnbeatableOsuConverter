namespace UnbeatableConverter.GUI;

public class FormatUtil
{
    /// <summary>Get the difficulty name (in [...]) from the file name.</summary>
    public static string ExtractDifficulty(string fileName)
    {
        var startIndex = fileName.LastIndexOf('[');
        var endIndex = fileName.LastIndexOf(']');

        if (startIndex == -1 || endIndex == -1 || endIndex <= startIndex)
            return string.Empty;

        return fileName.Substring(startIndex, endIndex - startIndex + 1);
    }
}

