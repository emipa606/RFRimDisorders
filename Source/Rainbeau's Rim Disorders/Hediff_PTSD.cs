using Verse;
using Verse.AI;

namespace RimDisorders;

public class Hediff_PTSD : MentalIllness
{
    protected override void DoSeverityAction(int index)
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

        if (index >= 2)
        {
            if (Rand.Value < 0.001f * index && GenAI.EnemyIsNear(pawn, 24f))
            {
                if (pawn.MentalState == null ||
                    pawn.MentalState.def != DiseaseDefOfRimDisorders.PanicAttack)
                {
                    if (pawn.health.hediffSet.HasHediff(DiseaseDefOfRimDisorders.Refractory))
                    {
                        return;
                    }

                    pawn.mindState.mentalStateHandler.TryStartMentalState(
                        DiseaseDefOfRimDisorders.PanicAttack, "RRD.StressFlashback".Translate(), true);
                }
            }
        }

        if (index < 3)
        {
            return;
        }

        if (!(Rand.Value < 1E-05f))
        {
            return;
        }

        if (pawn.MentalState != null && pawn.MentalState.def == DiseaseDefOfRimDisorders.PanicAttack)
        {
            return;
        }

        if (pawn.health.hediffSet.HasHediff(DiseaseDefOfRimDisorders.Refractory))
        {
            return;
        }

        pawn.mindState.mentalStateHandler.TryStartMentalState(
            DiseaseDefOfRimDisorders.PanicAttack, "RRD.RandomFlashback".Translate(), true);
    }
}