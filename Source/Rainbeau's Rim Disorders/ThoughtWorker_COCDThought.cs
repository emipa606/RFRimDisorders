using RimWorld;
using Verse;

namespace RimDisorders
{
    public class ThoughtWorker_COCDThought : ThoughtWorker
    {
        protected override ThoughtState CurrentStateInternal(Pawn p)
        {
            ThoughtState inactive;
            if (!p.health.hediffSet.HasHediff(DiseaseDefOfRimDisorders.COCD))
            {
                inactive = ThoughtState.Inactive;
            }
            else
            {
                var firstHediffOfDef =
                    (MentalIllness)p.health.hediffSet.GetFirstHediffOfDef(DiseaseDefOfRimDisorders.COCD);
                var curStageIndex = firstHediffOfDef.CurStageIndex;
                if (curStageIndex != 0)
                {
                    inactive = p.GetRoom(RegionType.Set_Passable).GetStat(RoomStatDefOf.Cleanliness) < 0f
                        ? ThoughtState.ActiveAtStage(curStageIndex - 1)
                        : ThoughtState.Inactive;
                }
                else
                {
                    inactive = ThoughtState.Inactive;
                }
            }

            return inactive;
        }
    }
}