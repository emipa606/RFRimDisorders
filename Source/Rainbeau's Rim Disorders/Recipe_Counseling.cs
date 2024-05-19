using System.Collections.Generic;
using RimWorld;
using Verse;

namespace RimDisorders;

public class Recipe_Counseling : RecipeWorker
{
    public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients,
        Bill bill)
    {
        base.ApplyOnPawn(pawn, part, billDoer, ingredients, bill);
        var str = recipe.defName;
        HediffDef majorDepression = null;
        switch (str)
        {
            case "CounselDepression":
                majorDepression = DiseaseDefOfRimDisorders.MajorDepression;
                break;
            case "CounselGAD":
                majorDepression = DiseaseDefOfRimDisorders.GeneralizedAnxiety;
                break;
            case "CounselPTSD":
                majorDepression = DiseaseDefOfRimDisorders.PTSD;
                break;
            case "CounselCOCD":
                majorDepression = DiseaseDefOfRimDisorders.COCD;
                break;
        }

        if (majorDepression == null)
        {
            return;
        }

        var firstHediffOfDef = (MentalIllness)pawn.health.hediffSet.GetFirstHediffOfDef(majorDepression);
        var relevantSkillLevel = pawn.RaceProps.IsMechanoid
            ? pawn.RaceProps.mechFixedSkillLevel
            : pawn.skills.GetSkill(SkillDefOf.Social).Level;
        firstHediffOfDef.Counsel(relevantSkillLevel);

        if (firstHediffOfDef.TryGetComp<HediffComp_MentalIllness>().Props.maxEpisodeStrength <
            firstHediffOfDef.def.stages[1].minSeverity)
        {
            pawn.health.RemoveHediff(firstHediffOfDef);
            Messages.Message($"{pawn.Name.ToStringShort} {"RRD.CuredText".Translate()}", pawn,
                MessageTypeDefOf.PositiveEvent);
        }

        pawn.health.AddHediff(HediffDef.Named("Counseled"));
    }

    public override string GetLabelWhenUsedOn(Pawn pawn, BodyPartRecord part)
    {
        string str;
        switch (recipe.defName)
        {
            case "CounselDepression":
                str = "RRD.CounselDepression".Translate();
                break;
            case "CounselGAD":
                str = "RRD.CounselAnxiety".Translate();
                break;
            default:
            {
                if (recipe.defName != "CounselPTSD")
                {
                    str = recipe.defName != "CounselCOCD" ? "" : "RRD.CounselCOCD".Translate().ToString();
                }
                else
                {
                    str = "RRD.CounselPTSD".Translate();
                }

                break;
            }
        }

        return str;
    }

    public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn, RecipeDef recipe)
    {
        IEnumerable<BodyPartRecord> bodyPartRecords;
        var bodyPartRecords1 = new List<BodyPartRecord>();
        if (!pawn.health.hediffSet.HasHediff(HediffDef.Named("Counseled")))
        {
            if (pawn.health.hediffSet.HasHediff(HediffDef.Named(recipe.removesHediff.defName)))
            {
                bodyPartRecords1.Add(pawn.health.hediffSet.GetBrain());
            }

            bodyPartRecords = bodyPartRecords1;
        }
        else
        {
            bodyPartRecords = bodyPartRecords1;
        }

        return bodyPartRecords;
    }
}