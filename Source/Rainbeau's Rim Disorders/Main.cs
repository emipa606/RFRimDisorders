using Verse;

namespace RimDisorders;

[StaticConstructorOnStartup]
public class Main
{
    private static readonly bool ageMattersLoaded;
    private static readonly bool cslLoaded;
    private static readonly HediffDef babyHediffDef;
    private static readonly HediffDef baby2HediffDef;
    private static readonly HediffDef toddlerHediffDef;
    private static readonly HediffDef childHediffDef;
    private static readonly HediffDef birthHediffDef;

    static Main()
    {
        ageMattersLoaded = ModLister.GetActiveModWithIdentifier("Troopersmith1.AgeMatters") != null;
        if (ageMattersLoaded)
        {
            Log.Message("[RimDisorders]: Adding compatibility patch for Age Matters");
            babyHediffDef = HediffDef.Named("AgeMatters_baby");
            baby2HediffDef = HediffDef.Named("AgeMatters_baby2");
            toddlerHediffDef = HediffDef.Named("AgeMatters_toddler");
            childHediffDef = HediffDef.Named("AgeMatters_child");
        }

        cslLoaded = ModLister.GetActiveModWithIdentifier("Dylan.CSL") != null;
        if (cslLoaded)
        {
            Log.Message("[RimDisorders]: Adding compatibility patch for Children School and Learning");
            birthHediffDef = HediffDef.Named("ChildBirthInProgress");
        }
    }

    public static bool ShouldIgnoreDownedPawn(Pawn pawn)
    {
        if (!ageMattersLoaded && !cslLoaded)
        {
            return false;
        }

        if (ageMattersLoaded)
        {
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
        }

        if (!cslLoaded)
        {
            return false;
        }

        return pawn.health.hediffSet.HasHediff(birthHediffDef);
    }
}