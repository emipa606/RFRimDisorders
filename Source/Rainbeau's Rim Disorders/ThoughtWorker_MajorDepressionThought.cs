using RimWorld;
using Verse;

namespace RimDisorders
{
    public class ThoughtWorker_MajorDepressionThought : ThoughtWorker
    {
        protected override ThoughtState CurrentStateInternal(Pawn p)
        {
            ThoughtState inactive;
            if (!p.health.hediffSet.HasHediff(DiseaseDefOfRimDisorders.MajorDepression))
            {
                inactive = ThoughtState.Inactive;
            }
            else
            {
                var firstHediffOfDef =
                    (MentalIllness)p.health.hediffSet.GetFirstHediffOfDef(DiseaseDefOfRimDisorders.MajorDepression);
                var curStageIndex = firstHediffOfDef.CurStageIndex;
                inactive = curStageIndex != 0 ? ThoughtState.ActiveAtStage(curStageIndex - 1) : ThoughtState.Inactive;
            }

            return inactive;
        }
    }
}