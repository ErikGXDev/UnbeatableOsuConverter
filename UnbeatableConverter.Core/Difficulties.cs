namespace UnbeatableConverter.Core;

public class Difficulties
{
    public static string[] DifficultyNames = new[]
    {
        "Beginner",
        "Normal",
        "Hard",
        "Expert",
        "UNBEATABLE",
        "Star"
    };

    public static string FromLast(int indexFromLast)
    {
        if (indexFromLast < 0 || indexFromLast >= DifficultyNames.Length)
            throw new ArgumentOutOfRangeException(nameof(indexFromLast), "Index is out of range.");

        return DifficultyNames[DifficultyNames.Length - 1 - indexFromLast];
    }

    public static string FormatDifficulty(string fileName, string difficultyName, string newDifficultyName)
    {
        var entryName = fileName.Replace("[" + difficultyName + "]",
            "[" + newDifficultyName + "]");

        return entryName;
    }
}