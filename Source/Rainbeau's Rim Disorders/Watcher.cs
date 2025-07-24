using Verse;

namespace RimDisorders;

public class Watcher(Map map) : MapComponent(map)
{
    public override void MapComponentTick()
    {
        base.MapComponentTick();
        MentalIllnessGiver.CheckAllPawnsForTriggers();
    }
}