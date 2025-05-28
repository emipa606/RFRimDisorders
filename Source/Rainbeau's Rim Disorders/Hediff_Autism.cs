using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace RimDisorders;

public class Hediff_Autism : MentalIllness
{
    protected override void DoSeverityAction(int index)
    {
        if (index == 0 || pawn.InMentalState || !pawn.Awake() || !pawn.Spawned)
        {
            return;
        }

        var partyDef = DefDatabase<DutyDef>.GetNamedSilentFail("Party");
        switch (index)
        {
            case 1:
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
            case 2:
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
            case 3:
            {
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
                break;
            }
        }
    }
}