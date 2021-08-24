using Verse;

namespace RimDisorders
{
    public class Watcher : MapComponent
    {
        private int currentTick;

        public Watcher(Map map) : base(map)
        {
        }

        public override void MapComponentTick()
        {
            base.MapComponentTick();
            currentTick = Find.TickManager.TicksGame;
            if (currentTick % 100 == 99)
            {
                MentalIllnessGiver.CheckAllPawnsForTriggers();
            }
        }
    }
}