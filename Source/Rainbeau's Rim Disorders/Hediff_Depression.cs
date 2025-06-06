﻿using RimWorld;
using Verse;

namespace RimDisorders;

public class Hediff_Depression : MentalIllness
{
    protected override void DoSeverityAction(int index)
    {
        if (pawn.mindState.mentalStateHandler.InMentalState)
        {
            return;
        }

        if (!pawn.Awake())
        {
            return;
        }

        if (!pawn.Spawned)
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

    public override void Tick()
    {
        base.Tick();
        if (!Main.mmLoaded)
        {
            return;
        }

        if (GenTicks.TicksAbs % GenTicks.TickRareInterval != 0)
        {
            return;
        }

        if (CurStageIndex == 0)
        {
            return;
        }

        if (pawn.health.hediffSet.HasHediff(HediffDef.Named("Soothingpills")))
        {
            Severity = 0.13f;
        }
    }
}