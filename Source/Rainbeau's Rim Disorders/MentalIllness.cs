using System.Collections.Generic;
using RimWorld;
using Verse;

namespace RimDisorders;

public abstract class MentalIllness : HediffWithComps
{
    public static float mult_counsel;

    static MentalIllness()
    {
        mult_counsel = 2f * (Controller.Settings.counselingEffectiveness / 100);
    }

    public virtual void AnyPostTickAction()
    {
        TryAddCounseling();
        DoSeverityAction(CurStageIndex);
    }

    public void Counsel(int social)
    {
        var hediffCompMentalIllness = this.TryGetComp<HediffComp_MentalIllness>();
        if (!hediffCompMentalIllness.Props.counselable)
        {
            return;
        }

        var value = Rand.Value * (2f + (0.8f * social)) / 100f;
        value *= mult_counsel;
        var mentalIllness = this;
        mentalIllness.Severity -= value;
        hediffCompMentalIllness.Props.maxEpisodeStrength -= value;
        var position = pawn.Position;
        MoteMaker.ThrowText(position.ToVector3(), pawn.Map,
            $"{"RRD.Improvement".Translate()}: {value * 100f:###}%", 3f);
    }

    public abstract void DoSeverityAction(int index);

    public override void PostTick()
    {
        base.PostTick();
        if (pawn != null)
        {
            AnyPostTickAction();
        }
        else
        {
            Log.Message("Cannot find pawn for hediff");
        }
    }

    public virtual void TryAddCounseling()
    {
        var hediffCompMentalIllness = this.TryGetComp<HediffComp_MentalIllness>();
        if (!pawn.Spawned)
        {
            return;
        }

        if (pawn.Map == null)
        {
            return;
        }

        if (hediffCompMentalIllness == null)
        {
            return;
        }

        if (CurStageIndex == 0)
        {
            return;
        }

        if (!hediffCompMentalIllness.Props.counselable)
        {
            return;
        }

        if (pawn.health.hediffSet.HasHediff(HediffDef.Named("Counseled")))
        {
            return;
        }

        RecipeDef counselDepression = null;
        if (def == DiseaseDefOfRimDisorders.MajorDepression)
        {
            counselDepression = DiseaseDefOfRimDisorders.CounselDepression;
        }

        if (def == DiseaseDefOfRimDisorders.GeneralizedAnxiety)
        {
            counselDepression = DiseaseDefOfRimDisorders.CounselGAD;
        }

        if (def == DiseaseDefOfRimDisorders.PTSD)
        {
            counselDepression = DiseaseDefOfRimDisorders.CounselPTSD;
        }

        if (def == DiseaseDefOfRimDisorders.COCD)
        {
            counselDepression = DiseaseDefOfRimDisorders.CounselCOCD;
        }

        if (counselDepression == null)
        {
            return;
        }

        var canBeCounseled = false;
        foreach (var freeColonistsSpawned in pawn.Map.mapPawns.FreeColonistsSpawned)
        {
            if (freeColonistsSpawned == pawn)
            {
                continue;
            }

            if (freeColonistsSpawned.Downed)
            {
                continue;
            }

            if (freeColonistsSpawned.Dead)
            {
                continue;
            }

            if (!counselDepression.PawnSatisfiesSkillRequirements(
                    freeColonistsSpawned))
            {
                continue;
            }

            canBeCounseled = true;
            break;
        }

        if (!canBeCounseled && pawn.BillStack.Bills.Exists(b =>
                b.recipe.defName == counselDepression.defName))
        {
            var bills = new List<Bill>();
            foreach (var bill in pawn.BillStack.Bills)
            {
                if (bill.recipe.defName == counselDepression.defName)
                {
                    bills.Add(bill);
                }
            }

            foreach (var bill1 in bills)
            {
                pawn.BillStack.Bills.Remove(bill1);
            }
        }
        else if (canBeCounseled)
        {
            if (pawn.BillStack.Bills.Exists(b =>
                    b.recipe.defName == counselDepression.defName))
            {
                return;
            }

            var billMedical = new Bill_Medical(counselDepression, null);
            pawn.BillStack.AddBill(billMedical);
            billMedical.Part = pawn.health.hediffSet.GetBrain();
        }
    }
}