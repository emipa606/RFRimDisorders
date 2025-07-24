using System.Linq;
using RimWorld;
using Verse;

namespace RimDisorders;

public static class MentalIllnessGiver
{
    private static readonly float rateDepression;
    private static readonly float rateAnxiety;
    private static readonly float rateOcd;
    private static readonly float ratePtsd;

    static MentalIllnessGiver()
    {
        rateDepression = 0.25f * (Controller.Settings.depressionChance / 100);
        rateAnxiety = 0.25f * (Controller.Settings.anxietyChance / 100);
        rateOcd = 0.25f * (Controller.Settings.cocdChance / 100);
        ratePtsd = 0.25f * (Controller.Settings.ptsdChance / 100);
    }

    public static void CheckAllPawnsForTriggers()
    {
        foreach (var map in Find.Maps)
        {
            foreach (var pawn in map.mapPawns.FreeColonistsAndPrisonersSpawned.ToList())
            {
                if (!pawn.Dead && pawn.IsHashIntervalTick(100))
                {
                    CheckPawnForTriggers(pawn);
                }
            }
        }
    }

    private static void CheckPawnForTriggers(Pawn pawn)
    {
        if (pawn.Dead)
        {
            return;
        }

        if (Main.ShouldIgnoreDownedPawn(pawn))
        {
            return;
        }

        var downed = pawn.Downed;
        var upper = false;
        var downer = false;
        foreach (var hediff in pawn.health.hediffSet.hediffs)
        {
            if (hediff is not Hediff_Addiction hediffAddiction)
            {
                continue;
            }

            if (!AddictionUtility.IsAddicted(pawn, hediffAddiction.Chemical))
            {
                continue;
            }

            switch (hediffAddiction.Chemical.defName)
            {
                case "Alcohol":
                case "Smokeleaf":
                    downer = true;
                    break;
                case "WakeUp":
                case "GoJuice":
                case "Psychite":
                    upper = true;
                    break;
            }
        }

        //bool flag4 = false;
        var isSick = false;
        var lowMood = false;
        if (pawn.health != null)
        {
            if (pawn.health.InPainShock)
            {
                //flag4 = true;
            }

            if (pawn.health.hediffSet is { AnyHediffMakesSickThought: true })
            {
                isSick = true;
            }
        }

        var lowStress = false;
        var mediumStress = false;
        var highStress = false;
        var criticalStress = false;
        foreach (var memory in pawn.needs.mood.thoughts.memories.Memories)
        {
            if (memory.def == ThoughtDefOf.WitnessedDeathAlly ||
                memory.def == ThoughtDefOf.WitnessedDeathFamily ||
                memory.def == ThoughtDefOf.WitnessedDeathNonAlly && pawn.story.traits.HasTrait(TraitDefOf.Kind))
            {
                lowStress = true;
                criticalStress = true;
            }

            if (memory.def == ThoughtDef.Named("ObservedLayingCorpse"))
            {
                highStress = true;
                lowStress = true;
            }

            if (memory.def == ThoughtDef.Named("ObservedLayingRottingCorpse"))
            {
                highStress = true;
                mediumStress = true;
                lowStress = true;
            }

            if (memory.MoodOffset() <= -20f)
            {
                lowMood = true;
            }
        }

        DamageInfo? nullable;
        var nerveDef = DefDatabase<TraitDef>.GetNamedSilentFail("Nerves");
        if (pawn.needs.mood.thoughts.TotalMoodOffset() < pawn.mindState.mentalBreaker.BreakThresholdMinor ||
            Rand.Value < 0.1f)
        {
            var rateAnx = 3.5E-05f * rateAnxiety;
            if (pawn.needs.mood.thoughts.TotalMoodOffset() < pawn.mindState.mentalBreaker.BreakThresholdMajor)
            {
                rateAnx *= 2f;
            }

            if (upper)
            {
                rateAnx *= 3f;
            }

            if (lowMood)
            {
                rateAnx *= 3f;
            }

            if (pawn.story.traits.HasTrait(nerveDef))
            {
                if (pawn.story.traits.GetTrait(nerveDef).Degree == 1)
                {
                    rateAnx /= 5f;
                }

                if (pawn.story.traits.GetTrait(nerveDef).Degree == 2)
                {
                    rateAnx /= 16f;
                }
            }

            if (Rand.Value < rateAnx &&
                !pawn.health.hediffSet.HasHediff(DiseaseDefOfRimDisorders.GeneralizedAnxiety))
            {
                nullable = null;
                pawn.health.AddHediff(DiseaseDefOfRimDisorders.GeneralizedAnxiety, null, nullable);
                var str = $"{pawn.Name.ToStringShort} {"RRD.DevelopedGAD".Translate()}";
                if (!upper)
                {
                    str = !(pawn.needs.mood.thoughts.TotalMoodOffset() <
                            pawn.mindState.mentalBreaker.BreakThresholdMajor)
                        ? $"{str}."
                        : string.Concat(str,
                            pawn.gender != Gender.Male ? "RRD.StressF".Translate() : "RRD.StressM".Translate());
                }
                else
                {
                    str = string.Concat(str, "RRD.StimAbuse".Translate());
                }

                Find.LetterStack.ReceiveLetter("RRD.Anxiety".Translate(), str, LetterDefOf.NegativeEvent);
            }
        }

        if (pawn.needs.mood.thoughts.TotalMoodOffset() < pawn.mindState.mentalBreaker.BreakThresholdMinor ||
            lowStress ||
            Rand.Value < 0.1f)
        {
            var rateDep = 3.5E-05f * rateDepression;
            if (pawn.needs.mood.thoughts.TotalMoodOffset() < pawn.mindState.mentalBreaker.BreakThresholdMajor)
            {
                rateDep *= 2f;
            }

            if (lowMood)
            {
                rateDep *= 2f;
            }

            if (downer)
            {
                rateDep *= 2f;
            }

            if (pawn.story.traits.HasTrait(nerveDef))
            {
                if (pawn.story.traits.GetTrait(nerveDef).Degree == 1)
                {
                    rateDep /= 5f;
                }

                if (pawn.story.traits.GetTrait(nerveDef).Degree == 2)
                {
                    rateDep /= 16f;
                }
            }

            if (Rand.Value < rateDep && !pawn.health.hediffSet.HasHediff(DiseaseDefOfRimDisorders.MajorDepression))
            {
                nullable = null;
                pawn.health.AddHediff(DiseaseDefOfRimDisorders.MajorDepression, null, nullable);
                var str1 = $"{pawn.Name.ToStringShort} {"RRD.DevelopedDepression".Translate()}";
                if (lowStress)
                {
                    if (!criticalStress)
                    {
                        str1 = !highStress
                            ? $"{str1}."
                            : string.Concat(str1, "RRD.SeenCorpse".Translate());
                    }
                    else
                    {
                        str1 = string.Concat(str1,
                            pawn.gender != Gender.Male
                                ? "RRD.WitnessedDeathF".Translate()
                                : "RRD.WitnessedDeathM".Translate());
                    }
                }
                else if (!(pawn.needs.mood.thoughts.TotalMoodOffset() <
                           pawn.mindState.mentalBreaker.BreakThresholdMajor))
                {
                    str1 = !downer
                        ? $"{str1}."
                        : string.Concat(str1, "RRD.DepressantAddiction".Translate());
                }
                else
                {
                    str1 = string.Concat(str1,
                        pawn.gender != Gender.Male ? "RRD.StressF".Translate() : "RRD.StressM".Translate());
                }

                Find.LetterStack.ReceiveLetter("RRD.Depression".Translate(), str1, LetterDefOf.NegativeEvent);
            }
        }

        if (pawn.needs.mood.thoughts.TotalMoodOffset() < pawn.mindState.mentalBreaker.BreakThresholdMinor ||
            mediumStress || isSick || Rand.Value < 0.05f)
        {
            var ocd = 2.5E-05f * rateOcd;
            if (pawn.needs.mood.thoughts.TotalMoodOffset() < pawn.mindState.mentalBreaker.BreakThresholdMajor)
            {
                ocd *= 1.5f;
            }

            if (upper)
            {
                ocd *= 1.2f;
            }

            if (lowMood)
            {
                ocd *= 1.4f;
            }

            if (mediumStress)
            {
                ocd *= 2f;
            }

            if (pawn.story.traits.HasTrait(nerveDef))
            {
                if (pawn.story.traits.GetTrait(nerveDef).Degree == 1)
                {
                    ocd /= 5f;
                }

                if (pawn.story.traits.GetTrait(nerveDef).Degree == 2)
                {
                    ocd /= 16f;
                }
            }

            if (Rand.Value < ocd && !pawn.health.hediffSet.HasHediff(DiseaseDefOfRimDisorders.COCD))
            {
                nullable = null;
                pawn.health.AddHediff(DiseaseDefOfRimDisorders.COCD, null, nullable);
                var str2 = string.Format(string.Concat("{0} ", "RRD.DevelopedCOCD".Translate()),
                    pawn.Name.ToStringShort);
                if (mediumStress)
                {
                    str2 = string.Concat(str2, "RRD.GrossMortality".Translate());
                }
                else if (!isSick)
                {
                    str2 = !(pawn.needs.mood.thoughts.TotalMoodOffset() <
                             pawn.mindState.mentalBreaker.BreakThresholdMajor)
                        ? $"{str2}."
                        : string.Concat(str2,
                            pawn.gender != Gender.Male ? "RRD.StressF".Translate() : "RRD.StressM".Translate());
                }
                else
                {
                    str2 = string.Concat(str2, "RRD.AfterSick".Translate());
                }

                Find.LetterStack.ReceiveLetter("RRD.COCD".Translate(), str2, LetterDefOf.NegativeEvent);
            }
        }

        if ((!criticalStress || !(Rand.Value < 0.05f)) && !downed)
        {
            return;
        }

        var ratePts = 0.0025f * ratePtsd;
        if (lowMood)
        {
            ratePts *= 2f;
        }

        if (pawn.story.traits.HasTrait(nerveDef))
        {
            if (pawn.story.traits.GetTrait(nerveDef).Degree == 1)
            {
                ratePts /= 5f;
            }

            if (pawn.story.traits.GetTrait(nerveDef).Degree == 2)
            {
                ratePts /= 16f;
            }
        }

        if (!(Rand.Value < ratePts) || pawn.health.hediffSet.HasHediff(DiseaseDefOfRimDisorders.PTSD))
        {
            return;
        }

        nullable = null;
        pawn.health.AddHediff(DiseaseDefOfRimDisorders.PTSD, null, nullable);
        var str3 = string.Format(string.Concat("{0}", "RRD.DevelopedPTSD".Translate()),
            pawn.Name.ToStringShort);
        if (!criticalStress)
        {
            str3 = !downed
                ? $"{str3}."
                : string.Concat(str3, "RRD.AfterDowned".Translate());
        }
        else
        {
            str3 = string.Concat(str3, "RRD.WitnessedTraumaticDeath".Translate());
        }

        Find.LetterStack.ReceiveLetter("RRD.PTSD".Translate(), str3, LetterDefOf.NegativeEvent);
    }
}