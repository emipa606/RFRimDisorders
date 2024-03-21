using Verse;

namespace RimDisorders;

public class Watcher(Map map) : MapComponent(map)
{
    private int currentTick;

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