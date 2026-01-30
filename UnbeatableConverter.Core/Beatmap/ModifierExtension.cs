using osu.Game.Audio;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Types;

namespace UnbeatableConverter.Core.Beatmap;

public enum NoteModifier
{
    Normal,
    Spike,
    Double,
    Zoom
}

public static class ModifierExtension
{
    struct ModifierInfo
    {
        public string Sample;
    }

    private static readonly Dictionary<NoteModifier, ModifierInfo> ModifierInfos = new()
    {
        { NoteModifier.Normal, new ModifierInfo { Sample = HitSampleInfo.HIT_WHISTLE } },
        { NoteModifier.Spike, new ModifierInfo { Sample = HitSampleInfo.HIT_CLAP } },
        { NoteModifier.Double, new ModifierInfo { Sample = HitSampleInfo.HIT_WHISTLE } },
        { NoteModifier.Zoom, new ModifierInfo { Sample = HitSampleInfo.HIT_WHISTLE } }
    };


    public static void ApplyModifier(this HitObject hitObject, NoteModifier modifier)
    {
        hitObject.Samples.Clear();

        var info = ModifierInfos[modifier];

        hitObject.Samples.Add(
            new HitSampleInfo(info.Sample)
        );
    }
}