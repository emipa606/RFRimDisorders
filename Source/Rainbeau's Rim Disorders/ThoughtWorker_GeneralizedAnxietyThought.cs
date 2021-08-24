using RimWorld;
using Verse;

namespace RimDisorders
{
    public class ThoughtWorker_GeneralizedAnxietyThought : ThoughtWorker
    {
        protected override ThoughtState CurrentStateInternal(Pawn p)
        {
            ThoughtState inactive;
            if (!p.health.hediffSet.HasHediff(DiseaseDefOfRimDisorders.GeneralizedAnxiety))
            {
                inactive = ThoughtState.Inactive;
            }
            else
            {
                var firstHediffOfDef =
                    (MentalIllness)p.health.hediffSet.GetFirstHediffOfDef(DiseaseDefOfRimDisorders.GeneralizedAnxiety);
                var curStageIndex = firstHediffOfDef.CurStageIndex;
                inactive = curStageIndex != 0 ? ThoughtState.ActiveAtStage(curStageIndex - 1) : ThoughtState.Inactive;
            }

            return inactive;
        }
    }
}