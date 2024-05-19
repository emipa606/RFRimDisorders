using Verse;

namespace RimDisorders;

public class Hediff_COCD : MentalIllness
{
    public override void DoSeverityAction(int index)
    {
        if (pawn.IsPrisoner)
        {
            return;
        }

        if (!pawn.Spawned)
        {
            return;
        }

        if (pawn.mindState.mentalStateHandler.InMentalState)
        {
            return;
        }

        if (index < 2)
        {
            return;
        }

        if (!(Rand.Value < 7.5E-05f))
        {
            return;
        }

        if (pawn.health.hediffSet.HasHediff(DiseaseDefOfRimDisorders.Refractory))
        {
            return;
        }

        pawn.mindState.mentalStateHandler.TryStartMentalState(DiseaseDefOfRimDisorders
            .COCDCleaning);
    }
}