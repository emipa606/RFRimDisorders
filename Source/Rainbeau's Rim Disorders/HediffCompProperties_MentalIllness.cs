using Verse;

namespace RimDisorders;

public class HediffCompProperties_MentalIllness : HediffCompProperties
{
    public bool counselable = true;
    public float episodeFrequency = 0f;
    public float maxEpisodeDuration = 10f;
    public float maxEpisodeStrength;
    public float minEpisodeDuration = 2f;
    public float minEpisodeStrength;

    public HediffCompProperties_MentalIllness()
    {
        compClass = typeof(HediffComp_MentalIllness);
        maxEpisodeStrength = GetNewMaxStrength;
        minEpisodeStrength = GetMinStrength;
    }

    public float GetMinStrength => Rand.Value * maxEpisodeStrength;

    public float GetNewMaxStrength => MaxEpisodeStrengthCurve().Evaluate(Rand.Value);

    private SimpleCurve MaxEpisodeStrengthCurve()
    {
        var simpleCurve = new SimpleCurve();
        simpleCurve.Add(new CurvePoint(0f, 0.2f));
        simpleCurve.Add(new CurvePoint(0.8f, 0.5f));
        simpleCurve.Add(new CurvePoint(1f, 1f));
        return simpleCurve;
    }
}