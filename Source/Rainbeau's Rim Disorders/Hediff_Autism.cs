using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace RimDisorders;

public class Hediff_Autism : MentalIllness
{
    public override void DoSeverityAction(int index)
    {
        if (index == 0 || pawn.InMentalState || !pawn.Awake())
        {
            return;
        }

        var partyDef = DefDatabase<DutyDef>.GetNamedSilentFail("Party");
        if (index == 1)
        {
            if (pawn is { jobs.curJob.playerForced: true })
            {
                pawn.needs.mood.thoughts.memories.TryGainMemory(DiseaseDefOfRimDisorders.AutismForcedWork);
            }

            if (pawn.mindState is not { duty: not null } || pawn.mindState.duty.def != partyDef)
            {
                return;
            }

            if (!(Rand.Value < 0.0002f))
            {
                return;
            }

            pawn.mindState.duty = null;
            pawn.GetLord().ownedPawns.Remove(pawn);
            pawn.jobs.EndCurrentJob(JobCondition.Incompletable);

            return;
        }

        if (index == 2)
        {
            if (pawn is { jobs.curJob.playerForced: true })
            {
                pawn.needs.mood.thoughts.memories.TryGainMemory(DiseaseDefOfRimDisorders
                    .AutismForcedWorkSevere);
            }

            if (pawn.mindState is not { duty: not null } || pawn.mindState.duty.def != partyDef)
            {
                return;
            }

            pawn.mindState.duty = null;
            pawn.GetLord().ownedPawns.Remove(pawn);
            pawn.jobs.EndCurrentJob(JobCondition.Incompletable);

            return;
        }

        if (index != 3)
        {
            return;
        }

        if (pawn is { jobs.curJob.playerForced: true })
        {
            pawn.needs.mood.thoughts.memories.TryGainMemory(DiseaseDefOfRimDisorders
                .AutismForcedWorkSevere);
            pawn.jobs.EndCurrentJob(JobCondition.QueuedNoLongerValid);
        }

        if (pawn.mindState?.duty == null || pawn.mindState.duty.def != partyDef)
        {
            return;
        }

        pawn.mindState.duty = null;
        pawn.GetLord().ownedPawns.Remove(pawn);
        pawn.jobs.EndCurrentJob(JobCondition.Incompletable);
    }
}