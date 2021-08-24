using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace RimDisorders
{
    public class Hediff_Autism : MentalIllness
    {
        public override void DoSeverityAction(int index)
        {
            if (index == 0 || pawn.InMentalState || !pawn.Awake())
            {
                return;
            }

            if (index == 1)
            {
                if (pawn is { jobs: { curJob: { playerForced: true } } })
                {
                    pawn.needs.mood.thoughts.memories.TryGainMemory(DiseaseDefOfRimDisorders.AutismForcedWork);
                }

                if (pawn.mindState is { duty: { } } && pawn.mindState.duty.def == DutyDefOf.Party)
                {
                    if (Rand.Value < 0.0002f)
                    {
                        pawn.mindState.duty = null;
                        pawn.GetLord().ownedPawns.Remove(pawn);
                        pawn.jobs.EndCurrentJob(JobCondition.Incompletable);
                    }
                }
            }

            if (index == 2)
            {
                if (pawn is { jobs: { curJob: { playerForced: true } } })
                {
                    pawn.needs.mood.thoughts.memories.TryGainMemory(DiseaseDefOfRimDisorders
                        .AutismForcedWorkSevere);
                }

                if (pawn.mindState is { duty: { } } && pawn.mindState.duty.def == DutyDefOf.Party)
                {
                    pawn.mindState.duty = null;
                    pawn.GetLord().ownedPawns.Remove(pawn);
                    pawn.jobs.EndCurrentJob(JobCondition.Incompletable);
                }
            }

            if (index != 3)
            {
                return;
            }

            if (pawn is { jobs: { curJob: { playerForced: true } } })
            {
                pawn.needs.mood.thoughts.memories.TryGainMemory(DiseaseDefOfRimDisorders
                    .AutismForcedWorkSevere);
                pawn.jobs.EndCurrentJob(JobCondition.QueuedNoLongerValid);
            }

            if (pawn.mindState == null || pawn.mindState.duty == null || pawn.mindState.duty.def != DutyDefOf.Party)
            {
                return;
            }

            pawn.mindState.duty = null;
            pawn.GetLord().ownedPawns.Remove(pawn);
            pawn.jobs.EndCurrentJob(JobCondition.Incompletable);
        }
    }
}