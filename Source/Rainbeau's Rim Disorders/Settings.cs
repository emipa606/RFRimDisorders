using UnityEngine;
using Verse;

namespace RimDisorders
{
    public class Settings : ModSettings
    {
        public float anxietyChance = 100f;
        public float cocdChance = 100f;
        public float counselingEffectiveness = 100f;
        public float depressionChance = 100f;
        public float ptsdChance = 100f;

        public void DoWindowContents(Rect canvas)
        {
            var list = new Listing_Standard
            {
                ColumnWidth = canvas.width
            };
            list.Begin(canvas);
            list.Gap(24);
            Text.Font = GameFont.Tiny;
            list.Label("RRD.Overview".Translate());
            Text.Font = GameFont.Small;
            list.Gap(48);
            list.Label("RRD.AnxietyChance".Translate() + "  " + (int)anxietyChance + "%");
            anxietyChance = list.Slider(anxietyChance, 0f, 500.99f);
            list.Gap();
            list.Label("RRD.COCDChance".Translate() + "  " + (int)cocdChance + "%");
            cocdChance = list.Slider(cocdChance, 0f, 500.99f);
            list.Gap();
            list.Label("RRD.DepressionChance".Translate() + "  " + (int)depressionChance + "%");
            depressionChance = list.Slider(depressionChance, 0f, 500.99f);
            list.Gap();
            list.Label("RRD.PTSDChance".Translate() + "  " + (int)ptsdChance + "%");
            ptsdChance = list.Slider(ptsdChance, 0f, 500.99f);
            list.Gap(48);
            list.Label("RRD.CounselingEffectiveness".Translate() + "  " + (int)counselingEffectiveness + "%");
            counselingEffectiveness = list.Slider(counselingEffectiveness, 10f, 300.99f);
            list.Gap();
            list.End();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref anxietyChance, "anxietyChance", 100.0f);
            Scribe_Values.Look(ref cocdChance, "cocdChance", 100.0f);
            Scribe_Values.Look(ref depressionChance, "depressionChance", 100.0f);
            Scribe_Values.Look(ref ptsdChance, "ptsdChance", 100.0f);
            Scribe_Values.Look(ref counselingEffectiveness, "counselingEffectiveness", 100.0f);
        }
    }
}