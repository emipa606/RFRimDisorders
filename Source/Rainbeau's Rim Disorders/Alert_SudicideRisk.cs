using RimWorld;
using Verse;

namespace RimDisorders;

public class Alert_SudicideRisk : Alert_Critical
{
    public override TaggedString GetExplanation()
    {
        return new TaggedString("RRD.SuicideRiskDesc".Translate());
    }

    public override string GetLabel()
    {
        return "RRD.SuicideRisk".Translate();
    }

    public override AlertReport GetReport()
    {
        var suidiceRisk = GetSuidiceRisk();
        var alertReport = suidiceRisk != null ? AlertReport.CulpritIs(suidiceRisk) : false;

        return alertReport;
    }

    private Pawn GetSuidiceRisk()
    {
        foreach (var map in Find.Maps)
        {
            foreach (var freeColonistsAndPrisonersSpawned in map.mapPawns.FreeColonistsAndPrisonersSpawned)
            {
                if (!freeColonistsAndPrisonersSpawned.mindState.mentalBreaker.BreakMajorIsImminent)
                {
                    continue;
                }

                foreach (var hediff in freeColonistsAndPrisonersSpawned.health.hediffSet.hediffs)
                {
                    if (hediff.def != DiseaseDefOfRimDisorders.MajorDepression || hediff.CurStageIndex != 3)
                    {
                        continue;
                    }

                    return freeColonistsAndPrisonersSpawned;
                }
            }
        }

        return null;
    }
}