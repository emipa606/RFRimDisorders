using Verse;

namespace RimDisorders;

public class HediffCompProperties_MentalIllness : HediffCompProperties
{
    public readonly bool counselable = true;
    public readonly float episodeFrequency = 0f;
    public readonly float maxEpisodeDuration = 10f;
    public readonly float minEpisodeDuration = 2f;
    public readonly float minEpisodeStrength;
    public float maxEpisodeStrength;

    public HediffCompProperties_MentalIllness()
    {
        compClass = typeof(HediffComp_MentalIllness);
        maxEpisodeStrength = GetNewMaxStrength;
        minEpisodeStrength = GetMinStrength;
    }

    private float GetMinStrength => Rand.Value * maxEpisodeStrength;

    private float GetNewMaxStrength => maxEpisodeStrengthCurve().Evaluate(Rand.Value);

    private static SimpleCurve maxEpisodeStrengthCurve()
    {
        var simpleCurve = new SimpleCurve();
        simpleCurve.Add(new CurvePoint(0f, 0.2f));
        simpleCurve.Add(new CurvePoint(0.8f, 0.5f));
        simpleCurve.Add(new CurvePoint(1f, 1f));
        return simpleCurve;
    }
}