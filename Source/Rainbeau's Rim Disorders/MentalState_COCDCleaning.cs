using RimWorld;
using Verse;
using Verse.AI;

namespace RimDisorders;

public class MentalState_COCDCleaning : MentalState
{
    public override void MentalStateTick()
    {
        if (pawn.jobs.curJob.def != JobDefOf.Clean)
        {
            var filthInHomeArea = pawn.Map.listerFilthInHomeArea.FilthInHomeArea;
            if (filthInHomeArea.Count != 0)
            {
                var thing = filthInHomeArea.RandomElement();
                pawn.jobs.StartJob(new Job(JobDefOf.Clean, thing), JobCondition.InterruptForced);
            }
            else
            {
                pawn.jobs.StartJob(new Job(JobDefOf.Wait, pawn.Position), JobCondition.InterruptForced);
            }
        }

        base.MentalStateTick();
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