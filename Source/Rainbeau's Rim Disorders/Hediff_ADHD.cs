using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace RimDisorders;

public class Hediff_ADHD : MentalIllness
{
    public override void DoSeverityAction(int index)
    {
        if (index <= 0)
        {
            return;
        }

        if (!pawn.Awake())
        {
            return;
        }

        if (!(Rand.Value < index * 5E-05f))
        {
            return;
        }

        if (pawn.Drafted)
        {
            pawn.drafter.Drafted = false;
        }
        else if (pawn is { needs.joy.tolerances: not null, jobs.jobQueue: not null })
        {
            Log.Message("ADHD tick event");
            var joyJob = GetJoyJob();
            if (joyJob == null)
            {
                return;
            }

            joyJob.playerForced = true;
            joyJob.ignoreJoyTimeAssignment = true;
            pawn.jobs.jobQueue.EnqueueFirst(joyJob, JobTag.Idle);
            pawn.jobs.curDriver.EndJobWith(JobCondition.InterruptOptional);
        }
    }

    private Job GetJoyJob()
    {
        Job job;
        var allDefsListForReading = DefDatabase<JoyGiverDef>.AllDefsListForReading;
        var joyGiverDefs =
            from j in allDefsListForReading
            where j.Worker.MissingRequiredCapacity(pawn) == null
            select j;
        if (joyGiverDefs.Count() != 0)
        {
            var joyGiverDef = joyGiverDefs.RandomElement();
            job = joyGiverDef.Worker.TryGiveJob(pawn);
        }
        else
        {
            Log.Message("No joygiverdefs found for ADHD!");
            job = null;
        }

        return job;
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

        if (pawn.health.hediffSet.HasHediff(HediffDef.Named("AdhdMedication")))
        {
            Severity = 0.13f;
        }
    }
}