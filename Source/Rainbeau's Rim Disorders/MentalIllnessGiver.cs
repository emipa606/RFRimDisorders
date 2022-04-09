using System.Linq;
using RimWorld;
using Verse;

namespace RimDisorders;

public static class MentalIllnessGiver
{
    public static float rate_dep;
    public static float rate_anx;
    public static float rate_ocd;
    public static float rate_pts;

    static MentalIllnessGiver()
    {
        rate_dep = 0.25f * (Controller.Settings.depressionChance / 100);
        rate_anx = 0.25f * (Controller.Settings.anxietyChance / 100);
        rate_ocd = 0.25f * (Controller.Settings.cocdChance / 100);
        rate_pts = 0.25f * (Controller.Settings.ptsdChance / 100);
    }

    public static void CheckAllPawnsForTriggers()
    {
        foreach (var map in Find.Maps)
        {
            foreach (var list in map.mapPawns.FreeColonistsAndPrisonersSpawned.ToList())
            {
                if (!list.Dead)
                {
                    CheckPawnForTriggers(list);
                }
            }
        }
    }

    public static void CheckPawnForTriggers(Pawn p)
    {
        if (p.Dead)
        {
            return;
        }

        var downed = p.Downed;
        var upper = false;
        var downer = false;
        foreach (var hediff in p.health.hediffSet.hediffs)
        {
            if (hediff is not Hediff_Addiction hediffAddiction)
            {
                continue;
            }

            if (!AddictionUtility.IsAddicted(p, hediffAddiction.Chemical))
            {
                continue;
            }

            if (hediffAddiction.Chemical.defName == "Alcohol" ||
                hediffAddiction.Chemical.defName == "Smokeleaf")
            {
                downer = true;
            }

            if (hediffAddiction.Chemical.defName == "WakeUp" ||
                hediffAddiction.Chemical.defName == "GoJuice" ||
                hediffAddiction.Chemical.defName == "Psychite")
            {
                upper = true;
            }
        }

        //bool flag4 = false;
        var isSick = false;
        var lowMood = false;
        if (p.health != null)
        {
            if (p.health.InPainShock)
            {
                //flag4 = true;
            }

            if (p.health.hediffSet is { AnyHediffMakesSickThought: true })
            {
                isSick = true;
            }
        }

        var lowStress = false;
        var mediumStress = false;
        var highStress = false;
        var criticalStress = false;
        foreach (var memory in p.needs.mood.thoughts.memories.Memories)
        {
            if (memory.def == ThoughtDefOf.WitnessedDeathAlly ||
                memory.def == ThoughtDefOf.WitnessedDeathFamily ||
                memory.def == ThoughtDefOf.WitnessedDeathNonAlly && p.story.traits.HasTrait(TraitDefOf.Kind))
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
        if (p.needs.mood.thoughts.TotalMoodOffset() < p.mindState.mentalBreaker.BreakThresholdMinor ||
            Rand.Value < 0.1f)
        {
            var rateAnx = 3.5E-05f * rate_anx;
            if (p.needs.mood.thoughts.TotalMoodOffset() < p.mindState.mentalBreaker.BreakThresholdMajor)
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

            if (p.story.traits.HasTrait(TraitDefOf.Nerves))
            {
                if (p.story.traits.GetTrait(TraitDefOf.Nerves).Degree == 1)
                {
                    rateAnx /= 5f;
                }

                if (p.story.traits.GetTrait(TraitDefOf.Nerves).Degree == 2)
                {
                    rateAnx /= 16f;
                }
            }

            if (Rand.Value < rateAnx &&
                !p.health.hediffSet.HasHediff(DiseaseDefOfRimDisorders.GeneralizedAnxiety))
            {
                nullable = null;
                p.health.AddHediff(DiseaseDefOfRimDisorders.GeneralizedAnxiety, null, nullable);
                var str = $"{p.Name.ToStringShort} {"RRD.DevelopedGAD".Translate()}";
                if (!upper)
                {
                    str = !(p.needs.mood.thoughts.TotalMoodOffset() < p.mindState.mentalBreaker.BreakThresholdMajor)
                        ? string.Concat(str, ".")
                        : string.Concat(str,
                            p.gender != Gender.Male ? "RRD.StressF".Translate() : "RRD.StressM".Translate());
                }
                else
                {
                    str = string.Concat(str, "RRD.StimAbuse".Translate());
                }

                Find.LetterStack.ReceiveLetter("RRD.Anxiety".Translate(), str, LetterDefOf.NegativeEvent);
            }
        }

        if (p.needs.mood.thoughts.TotalMoodOffset() < p.mindState.mentalBreaker.BreakThresholdMinor || lowStress ||
            Rand.Value < 0.1f)
        {
            var rateDep = 3.5E-05f * rate_dep;
            if (p.needs.mood.thoughts.TotalMoodOffset() < p.mindState.mentalBreaker.BreakThresholdMajor)
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

            if (p.story.traits.HasTrait(TraitDefOf.Nerves))
            {
                if (p.story.traits.GetTrait(TraitDefOf.Nerves).Degree == 1)
                {
                    rateDep /= 5f;
                }

                if (p.story.traits.GetTrait(TraitDefOf.Nerves).Degree == 2)
                {
                    rateDep /= 16f;
                }
            }

            if (Rand.Value < rateDep && !p.health.hediffSet.HasHediff(DiseaseDefOfRimDisorders.MajorDepression))
            {
                nullable = null;
                p.health.AddHediff(DiseaseDefOfRimDisorders.MajorDepression, null, nullable);
                var str1 = $"{p.Name.ToStringShort} {"RRD.DevelopedDepression".Translate()}";
                if (lowStress)
                {
                    if (!criticalStress)
                    {
                        str1 = !highStress
                            ? string.Concat(str1, ".")
                            : string.Concat(str1, "RRD.SeenCorpse".Translate());
                    }
                    else
                    {
                        str1 = string.Concat(str1,
                            p.gender != Gender.Male
                                ? "RRD.WitnessedDeathF".Translate()
                                : "RRD.WitnessedDeathM".Translate());
                    }
                }
                else if (!(p.needs.mood.thoughts.TotalMoodOffset() < p.mindState.mentalBreaker.BreakThresholdMajor))
                {
                    str1 = !downer
                        ? string.Concat(str1, ".")
                        : string.Concat(str1, "RRD.DepressantAddiction".Translate());
                }
                else
                {
                    str1 = string.Concat(str1,
                        p.gender != Gender.Male ? "RRD.StressF".Translate() : "RRD.StressM".Translate());
                }

                Find.LetterStack.ReceiveLetter("RRD.Depression".Translate(), str1, LetterDefOf.NegativeEvent);
            }
        }

        if (p.needs.mood.thoughts.TotalMoodOffset() < p.mindState.mentalBreaker.BreakThresholdMinor ||
            mediumStress || isSick || Rand.Value < 0.05f)
        {
            var rateOcd = 2.5E-05f * rate_ocd;
            if (p.needs.mood.thoughts.TotalMoodOffset() < p.mindState.mentalBreaker.BreakThresholdMajor)
            {
                rateOcd *= 1.5f;
            }

            if (upper)
            {
                rateOcd *= 1.2f;
            }

            if (lowMood)
            {
                rateOcd *= 1.4f;
            }

            if (mediumStress)
            {
                rateOcd *= 2f;
            }

            if (p.story.traits.HasTrait(TraitDefOf.Nerves))
            {
                if (p.story.traits.GetTrait(TraitDefOf.Nerves).Degree == 1)
                {
                    rateOcd /= 5f;
                }

                if (p.story.traits.GetTrait(TraitDefOf.Nerves).Degree == 2)
                {
                    rateOcd /= 16f;
                }
            }

            if (Rand.Value < rateOcd && !p.health.hediffSet.HasHediff(DiseaseDefOfRimDisorders.COCD))
            {
                nullable = null;
                p.health.AddHediff(DiseaseDefOfRimDisorders.COCD, null, nullable);
                var str2 = string.Format(string.Concat("{0} ", "RRD.DevelopedCOCD".Translate()),
                    p.Name.ToStringShort);
                if (mediumStress)
                {
                    str2 = string.Concat(str2, "RRD.GrossMortality".Translate());
                }
                else if (!isSick)
                {
                    str2 = !(p.needs.mood.thoughts.TotalMoodOffset() <
                             p.mindState.mentalBreaker.BreakThresholdMajor)
                        ? string.Concat(str2, ".")
                        : string.Concat(str2,
                            p.gender != Gender.Male ? "RRD.StressF".Translate() : "RRD.StressM".Translate());
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

        var ratePts = 0.0025f * rate_pts;
        if (lowMood)
        {
            ratePts *= 2f;
        }

        if (p.story.traits.HasTrait(TraitDefOf.Nerves))
        {
            if (p.story.traits.GetTrait(TraitDefOf.Nerves).Degree == 1)
            {
                ratePts /= 5f;
            }

            if (p.story.traits.GetTrait(TraitDefOf.Nerves).Degree == 2)
            {
                ratePts /= 16f;
            }
        }

        if (!(Rand.Value < ratePts) || p.health.hediffSet.HasHediff(DiseaseDefOfRimDisorders.PTSD))
        {
            return;
        }

        nullable = null;
        p.health.AddHediff(DiseaseDefOfRimDisorders.PTSD, null, nullable);
        var str3 = string.Format(string.Concat("{0}", "RRD.DevelopedPTSD".Translate()),
            p.Name.ToStringShort);
        if (!criticalStress)
        {
            str3 = !downed
                ? string.Concat(str3, ".")
                : string.Concat(str3, "RRD.AfterDowned".Translate());
        }
        else
        {
            str3 = string.Concat(str3, "RRD.WitnessedTraumaticDeath".Translate());
        }

        Find.LetterStack.ReceiveLetter("RRD.PTSD".Translate(), str3, LetterDefOf.NegativeEvent);
    }
}