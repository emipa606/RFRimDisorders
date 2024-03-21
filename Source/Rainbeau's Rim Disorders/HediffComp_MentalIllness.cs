using System;
using Verse;

namespace RimDisorders;

public class HediffComp_MentalIllness : HediffComp
{
    private int episodeTicks;

    private float episodeChancePerDay => Props.episodeFrequency;

    public HediffCompProperties_MentalIllness Props => (HediffCompProperties_MentalIllness)props;

    public override void CompPostTick(ref float severityadjustment)
    {
        if (episodeChancePerDay == 0f)
        {
            return;
        }

        base.CompPostTick(ref severityadjustment);
        if (Rand.Value < episodeChancePerDay / 60000f)
        {
            TryTriggerEpisode();
        }

        if (parent.CurStageIndex <= 0)
        {
            return;
        }

        episodeTicks++;
        if (episodeTicks > Props.maxEpisodeDuration * 60000f)
        {
            EndEpisode();
        }
        else if (episodeTicks > Props.minEpisodeDuration * 60000f)
        {
            if (Rand.Value < 0.00015f)
            {
                EndEpisode();
            }
        }
    }

    public override void CompTended(float quality, float maxQuality, int batchPosition = 0)
    {
        if (!parent.def.tendable)
        {
            return;
        }

        base.CompTended(quality, maxQuality, batchPosition);
        if (quality > Rand.Value)
        {
            parent.Severity = Math.Max(parent.Severity - 0.2f, 0f);
        }
    }

    public void EndEpisode()
    {
        parent.Severity = 0.05f;
        episodeTicks = 0;
    }

    protected virtual void TryTriggerEpisode()
    {
        if (episodeChancePerDay == 0f)
        {
            return;
        }

        if (parent.CurStageIndex == 0)
        {
            Log.Message($"Triggering episode of {Def.defName} for{Pawn.Name.ToStringShort}");
            parent.Severity = Props.minEpisodeStrength +
                              (Rand.Value * (Props.maxEpisodeStrength - Props.minEpisodeStrength));
        }
        else if (parent.Severity < Props.maxEpisodeStrength)
        {
            var hediffWithComp = parent;
            hediffWithComp.Severity += Rand.Value * Props.maxEpisodeStrength;
            parent.Severity = Math.Min(parent.Severity, Props.maxEpisodeStrength);
            var hediffCompPropertiesMentalIllness = Props;
            hediffCompPropertiesMentalIllness.maxEpisodeStrength += Rand.Value * 0.2f;
        }
    }
}