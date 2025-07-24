using RimWorld;
using Verse;
using Verse.AI;

namespace RimDisorders;

public class MentalState_PanicAttack : MentalState
{
    public override void MentalStateTick(int delta)
    {
        if (pawn.needs.rest.CurCategory == RestCategory.Exhausted)
        {
            return;
        }

        switch (Rand.Value)
        {
            case < 0.008f when pawn.jobs.curJob.def == JobDefOf.FleeAndCower:
                pawn.jobs.StartJob(new Job(JobDefOf.FleeAndCower, pawn.Map.listerThings.AllThings.RandomElement()),
                    JobCondition.InterruptForced);
                break;
            case < 0.012f when pawn.jobs.curJob.def == JobDefOf.FleeAndCower:
                pawn.jobs.StartJob(new Job(JobDefOf.FleeAndCower, pawn.Position), JobCondition.InterruptForced);
                break;
        }

        if (pawn.jobs.curJob.def != JobDefOf.FleeAndCower)
        {
            pawn.jobs.StartJob(new Job(JobDefOf.FleeAndCower, pawn.Map.listerThings.AllThings.RandomElement()),
                JobCondition.InterruptForced);
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