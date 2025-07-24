using RimWorld;
using Verse;
using Verse.AI;

namespace RimDisorders;

public class MentalState_SuicideAttempt : MentalState
{
    public override void MentalStateTick(int delta)
    {
        if (pawn.jobs.curJob.def != JobDefOf.Wait_Combat)
        {
            pawn.jobs.StartJob(new Job(JobDefOf.Wait, pawn.Position), JobCondition.InterruptForced);
        }

        if (Rand.Value < 0.005f)
        {
            var brain = pawn.health.hediffSet.GetBrain();
            foreach (var allPart in pawn.RaceProps.body.AllParts)
            {
                if (allPart.def != DefDatabase<BodyPartDef>.GetNamedSilentFail("Neck"))
                {
                    continue;
                }

                brain = allPart;
                break;
            }

            if (Rand.Value >= 0.2f)
            {
                var damageInfo = new DamageInfo(DamageDefOf.Cut, 200, 200, -1f, pawn, brain);
                pawn.TakeDamage(damageInfo);
            }
            else
            {
                var damageInfo1 = new DamageInfo(DamageDefOf.Cut, 1, 1, -1f, pawn, brain);
                pawn.TakeDamage(damageInfo1);
            }
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