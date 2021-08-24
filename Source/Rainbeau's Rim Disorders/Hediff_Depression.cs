using RimWorld;
using Verse;

namespace RimDisorders
{
    public class Hediff_Depression : MentalIllness
    {
        public override void DoSeverityAction(int index)
        {
            if (pawn.mindState.mentalStateHandler.InMentalState)
            {
                return;
            }

            if (!pawn.Awake())
            {
                return;
            }

            if (pawn.health.hediffSet.HasHediff(DiseaseDefOfRimDisorders.Refractory))
            {
                return;
            }

            if (index >= 2)
            {
                if (Rand.Value < 5E-05f * index)
                {
                    pawn.mindState.mentalStateHandler.TryStartMentalState(DiseaseDefOfRimDisorders
                        .Binging_Food);
                    pawn.health.AddHediff(DiseaseDefOfRimDisorders.Refractory);
                }
            }

            if (index < 3)
            {
                return;
            }

            if (!pawn.mindState.mentalBreaker.BreakMajorIsImminent &&
                !pawn.mindState.mentalBreaker.BreakExtremeIsImminent)
            {
                return;
            }

            if (!(Rand.Value < 0.0001f))
            {
                return;
            }

            if (pawn.IsPrisoner)
            {
                return;
            }

            pawn.mindState.mentalStateHandler.TryStartMentalState(DiseaseDefOfRimDisorders
                .SuicideAttempt);
            pawn.health.AddHediff(DiseaseDefOfRimDisorders.Refractory);
        }
    }
}