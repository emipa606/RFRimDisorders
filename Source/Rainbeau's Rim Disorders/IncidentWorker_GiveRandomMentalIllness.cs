using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace RimDisorders;

public class IncidentWorker_GiveRandomMentalIllness : IncidentWorker
{
    private HediffDef RandomMentalIllness => DiseaseDefOfRimDisorders.MajorDepression;

    protected override bool CanFireNowSub(IncidentParms parms)
    {
        return true;
    }

    protected override bool TryExecuteWorker(IncidentParms parms)
    {
        var map = (Map)parms.target;
        var list = map.mapPawns.FreeColonistsAndPrisoners.ToList();
        if (list.Count == 0)
        {
            return false;
        }

        list.Shuffle();
        var hediffDefs = new List<HediffDef>
        {
            DiseaseDefOfRimDisorders.MajorDepression,
            DiseaseDefOfRimDisorders.GeneralizedAnxiety,
            DiseaseDefOfRimDisorders.COCD,
            DiseaseDefOfRimDisorders.PTSD
        };
        if (hediffDefs.Count == 0)
        {
            return false;
        }

        if (Rand.Value >= 1f)
        {
            return false;
        }

        list[0].health.AddHediff(hediffDefs.RandomElement());
        return true;
    }
}