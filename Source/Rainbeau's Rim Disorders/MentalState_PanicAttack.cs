using RimWorld;
using Verse;
using Verse.AI;

namespace RimDisorders
{
    public class MentalState_PanicAttack : MentalState
    {
        public override void MentalStateTick()
        {
            if (pawn.needs.rest.CurCategory == RestCategory.Exhausted)
            {
                return;
            }

            if (Rand.Value < 0.008f && pawn.jobs.curJob.def == JobDefOf.FleeAndCower)
            {
                pawn.jobs.StartJob(new Job(JobDefOf.FleeAndCower, pawn.Map.listerThings.AllThings.RandomElement()),
                    JobCondition.InterruptForced);
            }
            else if (Rand.Value < 0.012f && pawn.jobs.curJob.def == JobDefOf.FleeAndCower)
            {
                pawn.jobs.StartJob(new Job(JobDefOf.FleeAndCower, pawn.Position), JobCondition.InterruptForced);
            }

            if (pawn.jobs.curJob.def != JobDefOf.FleeAndCower)
            {
                pawn.jobs.StartJob(new Job(JobDefOf.FleeAndCower, pawn.Map.listerThings.AllThings.RandomElement()),
                    JobCondition.InterruptForced);
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
}