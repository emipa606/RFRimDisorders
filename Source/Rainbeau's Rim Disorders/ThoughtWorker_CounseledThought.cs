using RimWorld;
using Verse;

namespace RimDisorders
{
    public class ThoughtWorker_CounseledThought : ThoughtWorker
    {
        protected override ThoughtState CurrentStateInternal(Pawn p)
        {
            var thoughtState = !p.health.hediffSet.HasHediff(DiseaseDefOfRimDisorders.Counseled)
                ? ThoughtState.Inactive
                : ThoughtState.ActiveAtStage(0);
            return thoughtState;
        }
    }
}