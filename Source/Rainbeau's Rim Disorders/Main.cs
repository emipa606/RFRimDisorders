using Verse;

namespace RimDisorders;

[StaticConstructorOnStartup]
public class Main
{
    private static readonly bool ageMattersLoaded;
    private static readonly bool cslLoaded;
    public static readonly bool mmLoaded;
    public static readonly bool bcLoaded;
    private static readonly HediffDef babyHediffDef;
    private static readonly HediffDef baby2HediffDef;
    private static readonly HediffDef toddlerHediffDef;
    private static readonly HediffDef childHediffDef;
    private static readonly HediffDef birthHediffDef;
    private static readonly HediffDef babyStateHediffDef;
    private static readonly HediffDef abasiaHediffDef;
    private static readonly HediffDef deathRestHediffDef;

    static Main()
    {
        if (ModLister.RoyaltyInstalled)
        {
            abasiaHediffDef = HediffDef.Named("Abasia");
        }

        if (ModLister.BiotechInstalled)
        {
            deathRestHediffDef = HediffDef.Named("Deathrest");
        }

        ageMattersLoaded = ModLister.GetActiveModWithIdentifier("Troopersmith1.AgeMatters") != null;
        if (ageMattersLoaded)
        {
            Log.Message("[RimDisorders]: Adding compatibility patch for Age Matters");
            babyHediffDef = HediffDef.Named("AgeMatters_baby");
            baby2HediffDef = HediffDef.Named("AgeMatters_baby2");
            toddlerHediffDef = HediffDef.Named("AgeMatters_toddler");
            childHediffDef = HediffDef.Named("AgeMatters_child");
        }

        mmLoaded = ModLister.GetActiveModWithIdentifier("Techmago.MoreMedications") != null;
        bcLoaded = ModLister.GetActiveModWithIdentifier("babies.and.children.continued.13") != null;
        if (bcLoaded)
        {
            babyStateHediffDef = HediffDef.Named("BabyState0");
        }

        cslLoaded = ModLister.GetActiveModWithIdentifier("Dylan.CSL") != null;
        if (!cslLoaded)
        {
            return;
        }

        Log.Message("[RimDisorders]: Adding compatibility patch for Children School and Learning");
        birthHediffDef = HediffDef.Named("ChildBirthInProgress");
    }

    public static bool ShouldIgnoreDownedPawn(Pawn pawn)
    {
        if (!pawn.mindState.mentalBreaker.CanDoRandomMentalBreaks)
        {
            return true;
        }

        if (ModLister.RoyaltyInstalled && pawn.health.hediffSet.HasHediff(abasiaHediffDef))
        {
            return true;
        }

        if (ModLister.BiotechInstalled &&
            (pawn.DevelopmentalStage is DevelopmentalStage.Baby or DevelopmentalStage.Newborn ||
             pawn.health.hediffSet.HasHediff(deathRestHediffDef)))
        {
            return true;
        }

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

        if (!bcLoaded)
        {
            return cslLoaded && pawn.health.hediffSet.HasHediff(birthHediffDef);
        }

        if (!pawn.health.hediffSet.HasHediff(babyStateHediffDef))
        {
            return false;
        }

        return pawn.health.hediffSet.GetFirstHediffOfDef(babyStateHediffDef).CurStageIndex != 3;
    }
}