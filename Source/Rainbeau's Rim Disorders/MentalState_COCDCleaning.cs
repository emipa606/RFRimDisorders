﻿using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace RimDisorders;

public class MentalState_COCDCleaning : MentalState
{
    public override void MentalStateTick()
    {
        if (pawn.jobs.curJob?.def == JobDefOf.Clean)
        {
            base.MentalStateTick();
            return;
        }

        var filthInHomeArea = pawn.Map.listerFilthInHomeArea?.FilthInHomeArea.Where(thing =>
            pawn.CanReserveAndReach(thing, PathEndMode.ClosestTouch, Danger.Some));
        if (filthInHomeArea?.Any() == true)
        {
            pawn.jobs.StartJob(new Job(JobDefOf.Clean, filthInHomeArea.RandomElement())
            {
                locomotionUrgency = LocomotionUrgency.Sprint
            }, JobCondition.InterruptForced);
            base.MentalStateTick();
            return;
        }

        if (pawn.jobs.curJob?.def != JobDefOf.GotoWander)
        {
            var randomLocation = RCellFinder.RandomWanderDestFor(pawn, pawn.Position, 7f, null,
                PawnUtility.ResolveMaxDanger(pawn, Danger.Some));
            pawn.jobs.StartJob(new Job(JobDefOf.GotoWander, randomLocation), JobCondition.InterruptForced);
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