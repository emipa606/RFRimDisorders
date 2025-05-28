using RimWorld;
using Verse;

namespace RimDisorders;

public class ThoughtWorker_AutismCrowded : ThoughtWorker
{
    protected override ThoughtState CurrentStateInternal(Pawn p)
    {
        if (!p.health.hediffSet.HasHediff(DiseaseDefOfRimDisorders.Autism))
        {
            return ThoughtState.Inactive;
        }

        var firstHediffOfDef =
            (MentalIllness)p.health.hediffSet.GetFirstHediffOfDef(DiseaseDefOfRimDisorders.Autism);
        var curStageIndex = firstHediffOfDef.CurStageIndex;
        if (curStageIndex < 1 || p.GetRoom(RegionType.Set_Passable) == null)
        {
            return ThoughtState.Inactive;
        }

        var num = 0;
        foreach (var allPawnsSpawned in p.Map.mapPawns.AllPawnsSpawned)
        {
            if (!allPawnsSpawned.Awake() || !allPawnsSpawned.RaceProps.Humanlike ||
                allPawnsSpawned.GetRoom(RegionType.Set_Passable) != p.GetRoom(RegionType.Set_Passable))
            {
                continue;
            }

            if (allPawnsSpawned.Position.InHorDistOf(p.Position, 10f))
            {
                num++;
            }
        }

        switch (curStageIndex)
        {
            case 1 when num > 9:
                return ThoughtState.ActiveAtStage(0);
            case 2 when num > 5:
                return ThoughtState.ActiveAtStage(1);
        }

        if (curStageIndex != 3 || num <= 4)
        {
            return ThoughtState.Inactive;
        }

        return ThoughtState.ActiveAtStage(2);
    }
}