using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace RimDisorders;

public class MentalState_COCDCleaning : MentalState
{
    public override void MentalStateTick(int delta)
    {
        if (pawn.jobs.curJob?.def == JobDefOf.Clean)
        {
            base.MentalStateTick(delta);
            return;
        }

        var filthInHomeArea = pawn.Map.listerFilthInHomeArea?.FilthInHomeArea.Where(thing =>
            pawn.CanReserveAndReach(thing, PathEndMode.ClosestTouch, Danger.Some));
        if (filthInHomeArea != null)
        {
            var inHomeArea = filthInHomeArea as Thing[] ?? filthInHomeArea.ToArray();
            if (inHomeArea.Any())
            {
                var cleanJob = new Job(JobDefOf.Clean)
                {
                    locomotionUrgency = LocomotionUrgency.Sprint
                };
                cleanJob.AddQueuedTarget(TargetIndex.A, inHomeArea.RandomElement());
                pawn.jobs.StartJob(cleanJob, JobCondition.InterruptForced);
                base.MentalStateTick(delta);
                return;
            }
        }

        if (pawn.jobs.curJob?.def != JobDefOf.GotoWander)
        {
            var randomLocation = RCellFinder.RandomWanderDestFor(pawn, pawn.Position, 7f, null,
                PawnUtility.ResolveMaxDanger(pawn, Danger.Some));
            pawn.jobs.StartJob(new Job(JobDefOf.GotoWander, randomLocation), JobCondition.InterruptForced);
        }

        base.MentalStateTick(delta);
    }

    public override void PostEnd()
    {
        base.PostEnd();
        pawn.health.AddHediff(DiseaseDefOfRimDisorders.Refractory);
    }

    public override RandomSocialMode SocialModeMax()
    {
        return 0;
    }
}