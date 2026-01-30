using System.IO.Compression;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.Formats;
using osu.Game.IO;
using UnbeatableConverter.Core.Beatmap;

namespace UnbeatableConverter.Core;

public class OszExporter
{
    private string _inputFilePath;

    private HashSet<string> _beatmapEntries = new();
    private HashSet<string> _assetEntries = new();

    public HashSet<string> BeatmapEntries => _beatmapEntries;
    public HashSet<string> AssetEntries => _assetEntries;

    public OszExporter(string inputFilePath)
    {
        _inputFilePath = inputFilePath;

        ReadEntries();
    }

    private void ReadEntries()
    {
        using var zipStream = ZipFile.OpenRead(_inputFilePath);

        foreach (var entry in zipStream.Entries)
        {
            if (Util.IsBeatmapFile(entry.FullName))
            {
                _beatmapEntries.Add(entry.FullName);

                var stream = entry.Open();
                var lineReader = new LineBufferedReader(stream);
                var decoder = new LegacyBeatmapDecoder();
                var beatmap = decoder.Decode(lineReader);

                var audioFile = beatmap.BeatmapInfo.Metadata.AudioFile;
                if (!string.IsNullOrEmpty(audioFile))
                {
                    _assetEntries.Add(audioFile);
                }

                var backgroundFile = beatmap.BeatmapInfo.Metadata.BackgroundFile;
                if (!string.IsNullOrEmpty(backgroundFile))
                {
                    _assetEntries.Add(backgroundFile);
                }
            }
        }
    }

    // Generates the output file path based on the input file path
    // .../file.osz -> .../file_converted.osz
    private static readonly Random Random = new();

    private string GetOutputPath(string inputPath)
    {
        var directory = Path.GetDirectoryName(inputPath);
        var filenameWithoutExt = Path.GetFileNameWithoutExtension(inputPath);
        filenameWithoutExt += "_" + Random.Next();
        var outputFilename = $"{filenameWithoutExt}_converted.zip";
        return Path.Combine(directory ?? string.Empty, outputFilename);
    }


    private string FormatEntryName(string entryName, int index, string oldDifficultyName)
    {
        var originalEntryName = Path.GetFileNameWithoutExtension(entryName) + ".txt";

        var newDifficultyName = Difficulties.FromLast(index);

        var formattedName = Difficulties.FormatDifficulty(originalEntryName,
            oldDifficultyName,
            newDifficultyName);

        return formattedName;
    }

    /// Converts the .osz file, saves it to disk (in the same directory as the input osz), and returns the output file path
    public string ExportFull()
    {
        if (!File.Exists(_inputFilePath))
            throw new FileNotFoundException("Input file not found.", _inputFilePath);

        using var zipStream = ZipFile.OpenRead(_inputFilePath);

        var outputPath = GetOutputPath(_inputFilePath);
        using var outputZipStream = ZipFile.Open(outputPath, ZipArchiveMode.Create);

        // Convert beatmaps
        var index = 0;
        foreach (var beatmapEntryName in _beatmapEntries)
        {
            var entry = zipStream.GetEntry(beatmapEntryName);
            if (entry == null)
            {
                continue;
            }

            using var entryStream = entry.Open();

            // Convert beatmap
            var converter = new BeatmapConverter();
            var beatmap = converter.DecodeBeatmap(entryStream);

            var entryName = FormatEntryName(beatmapEntryName, index, beatmap.BeatmapInfo.DifficultyName);

            // Save output
            using var outputStream = outputZipStream.CreateEntry(entryName).Open();
            using var encodedStream = converter.EncodeBeatmap(beatmap);
            encodedStream.CopyTo(outputStream);

            index++;
        }

        // Copy asset files (audio, background)
        foreach (var assetEntryName in _assetEntries)
        {
            var assetEntry = zipStream.GetEntry(assetEntryName);
            if (assetEntry != null)
            {
                using var assetEntryStream = assetEntry.Open();
                using var newAssetEntryStream = outputZipStream.CreateEntry(assetEntryName).Open();
                assetEntryStream.CopyTo(newAssetEntryStream);
            }
        }

        return outputPath;
    }

    /// Converts a single beatmap entry from the .osz file and returns the output file path
    /// Copies its associated audio file as well, if present
    public string ExportSingle(string beatmapEntryName, string outputDirectory)
    {
        using var zipStream = ZipFile.OpenRead(_inputFilePath);

        var entry = zipStream.GetEntry(beatmapEntryName);
        if (entry == null)
        {
            throw new FileNotFoundException("Beatmap entry not found in the .osz file.", beatmapEntryName);
        }

        using var entryStream = entry.Open();

        // Convert beatmap
        var converter = new BeatmapConverter();
        var beatmap = converter.DecodeBeatmap(entryStream);

        var audioFile = beatmap.BeatmapInfo.Metadata.AudioFile;
        if (!string.IsNullOrEmpty(audioFile))
        {
            var audioEntry = zipStream.GetEntry(audioFile);
            if (audioEntry != null)
            {
                using var audioEntryStream = audioEntry.Open();
                var outputAudioPath = Path.Combine(outputDirectory, audioFile);
                using var outputAudioFileStream = File.Create(outputAudioPath);
                audioEntryStream.CopyTo(outputAudioFileStream);
            }
        }

        // Save output
        var formattedEntryName = FormatEntryName(beatmapEntryName, 0, beatmap.BeatmapInfo.DifficultyName);
        var outputPath = Path.Combine(outputDirectory, formattedEntryName);

        using var outputFileStream = File.Create(outputPath);
        using var encodedStream = converter.EncodeBeatmap(beatmap);
        encodedStream.CopyTo(outputFileStream);

        return outputPath;
    }
}