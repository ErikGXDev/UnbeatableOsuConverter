using osu.Game.Beatmaps;
using osu.Game.Rulesets.Mania.Beatmaps;
using osu.Game.Rulesets.Mania.Objects;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Types;

namespace UnbeatableConverter.Core.Beatmap;

public class DuplicateRemover
{
    // The issue is that with osu!mania maps, there are of course notes
    // that are meant to be hit at the same time, however this causes issues
    // with the conversion process, since for example 4 column maps that may
    // have multiple notes at the same time end up producing overlapping notes
    // when being compressed to 2 columns.
    // Therefore, notes, holds, etc. that are inside each other at the same time
    // or during the duration of another note should be removed.
    // However, notes that are "overlapping" but are on different columns should be kept.
    // Hold notes should still be considered for their duration.
    public void Convert(ManiaBeatmap beatmap)
    {
        var hitObjects = beatmap.HitObjects;
        hitObjects.Sort((x, y) => x.StartTime.CompareTo(y.StartTime));

        var toRemove = new HashSet<ManiaHitObject>();

        const double eps = 10.0; // 10ms tolerance


        for (int i = 0; i < hitObjects.Count; i++)
        {
            var objA = hitObjects[i];
            var endTimeA = objA.GetEndTime();

            for (int j = i + 1; j < hitObjects.Count; j++)
            {
                var objB = hitObjects[j];

                // If objB starts after objA ends, no need to check further
                if (objB.StartTime > endTimeA + eps)
                    break;

                var endTimeB = objB.GetEndTime();


                // Check for overlap in time with epsilon tolerance
                bool isOverlapping = objA.StartTime < endTimeB + eps && objB.StartTime < endTimeA + eps;


                if (isOverlapping)
                {
                    if (objA.Column == objB.Column)
                    {
                        // Mark objB for removal
                        toRemove.Add(objB);
                    }
                }
            }
        }

        foreach (var obj in toRemove)
        {
            beatmap.HitObjects.Remove(obj);
        }


        Console.WriteLine($"Removed {toRemove.Count} overlapping hit objects.");
    }
}