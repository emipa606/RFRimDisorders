using Verse;

namespace RimDisorders;

[StaticConstructorOnStartup]
public class Main
{
    private static readonly bool ageMattersLoaded;
    private static readonly HediffDef babyHediffDef;
    private static readonly HediffDef baby2HediffDef;
    private static readonly HediffDef toddlerHediffDef;
    private static readonly HediffDef childHediffDef;

    static Main()
    {
        ageMattersLoaded = ModLister.GetActiveModWithIdentifier("Troopersmith1.AgeMatters") != null;
        if (!ageMattersLoaded)
        {
            return;
        }

        Log.Message("[RimDisorders]: Adding compatibility patch for Age Matters");
        babyHediffDef = HediffDef.Named("AgeMatters_baby");
        baby2HediffDef = HediffDef.Named("AgeMatters_baby2");
        toddlerHediffDef = HediffDef.Named("AgeMatters_toddler");
        childHediffDef = HediffDef.Named("AgeMatters_child");
    }

    public static bool ShouldIgnoreDownedPawn(Pawn pawn)
    {
        if (!ageMattersLoaded)
        {
            return false;
        }

        if (pawn.health.hediffSet.HasHediff(babyHediffDef))
        {
            return true;
        }

        if (pawn.health.hediffSet.HasHediff(baby2HediffDef))
        {
            return true;
        }

        if (pawn.health.hediffSet.HasHediff(toddlerHediffDef))
        {
            return true;
        }

        if (pawn.health.hediffSet.HasHediff(childHediffDef))
        {
            return true;
        }

        return false;
    }
}