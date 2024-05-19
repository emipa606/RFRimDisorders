using RimWorld;
using Verse;

namespace RimDisorders;

public class Hediff_GeneralizedAnxiety : MentalIllness
{
    public override void DoSeverityAction(int index)
    {
        if (pawn.mindState.mentalStateHandler.InMentalState)
        {
            return;
        }

        if (pawn.IsPrisoner)
        {
            return;
        }

        if (!pawn.Spawned)
        {
            return;
        }

        if (!pawn.Awake())
        {
            return;
        }

        if (index < 3)
        {
            return;
        }

        if (!(Rand.Value < 3.5E-05f))
        {
            return;
        }

        if (pawn.health.hediffSet.HasHediff(DiseaseDefOfRimDisorders.Refractory))
        {
            return;
        }

        pawn.mindState.mentalStateHandler.TryStartMentalState(DiseaseDefOfRimDisorders
            .PanicAttack);
    }
}