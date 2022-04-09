using Verse;

namespace RimDisorders;

[StaticConstructorOnStartup]
public class Main
{
    private static readonly bool ageMattersLoaded;
    private static readonly HediffDef babyHediffDef;

    static Main()
    {
        ageMattersLoaded = ModLister.GetActiveModWithIdentifier("Troopersmith1.AgeMatters") != null;
        if (ageMattersLoaded)
        {
            Log.Message("[RimDisorders]: Adding compatibility patch for Age Matters");
            babyHediffDef = HediffDef.Named("AgeMatters_baby");
        }
    }

    public static bool ShouldIgnoreDownedPawn(Pawn pawn)
    {
        if (!ageMattersLoaded)
        {
            return false;
        }

        return pawn.health.hediffSet.HasHediff(babyHediffDef);
    }
}