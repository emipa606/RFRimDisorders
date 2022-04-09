using RimWorld;
using Verse;

namespace RimDisorders;

public class ThoughtWorker_AutismCrowded : ThoughtWorker
{
    protected override ThoughtState CurrentStateInternal(Pawn p)
    {
        ThoughtState inactive;
        if (p.health.hediffSet.HasHediff(DiseaseDefOfRimDisorders.Autism))
        {
            var firstHediffOfDef =
                (MentalIllness)p.health.hediffSet.GetFirstHediffOfDef(DiseaseDefOfRimDisorders.Autism);
            var curStageIndex = firstHediffOfDef.CurStageIndex;
            if (curStageIndex < 1)
            {
                inactive = ThoughtState.Inactive;
            }
            else if (p.GetRoom(RegionType.Set_Passable) != null)
            {
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

                if (curStageIndex == 1 && num > 9)
                {
                    inactive = ThoughtState.ActiveAtStage(0);
                }
                else if (curStageIndex != 2 || num <= 5)
                {
                    if (curStageIndex == 3 && num > 4)
                    {
                        inactive = ThoughtState.ActiveAtStage(2);
                        return inactive;
                    }

                    inactive = ThoughtState.Inactive;
                    return inactive;
                }
                else
                {
                    inactive = ThoughtState.ActiveAtStage(1);
                }
            }
            else
            {
                inactive = ThoughtState.Inactive;
            }
        }
        else
        {
            inactive = ThoughtState.Inactive;
            return inactive;
        }

        return inactive;
    }
}