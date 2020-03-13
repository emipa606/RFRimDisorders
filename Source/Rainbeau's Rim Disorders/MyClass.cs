using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace RimDisorders {

	public class Controller : Mod {
		public static Settings Settings;
		public override string SettingsCategory() { return "RRD.RimDisorders".Translate(); }
		public override void DoSettingsWindowContents(Rect canvas) { Settings.DoWindowContents(canvas); }
		public Controller(ModContentPack content) : base(content) {
			Settings = GetSettings<Settings>();
		}
	}

	public class Settings : ModSettings {
		public float anxietyChance = 100f;
		public float cocdChance = 100f;
		public float depressionChance = 100f;
		public float ptsdChance = 100f;
		public float counselingEffectiveness = 100f;
		public void DoWindowContents(Rect canvas) {
			Listing_Standard list = new Listing_Standard();
			list.ColumnWidth = canvas.width;
			list.Begin(canvas);
			list.Gap(24);
			Text.Font = GameFont.Tiny;
			list.Label("RRD.Overview".Translate());
			Text.Font = GameFont.Small;
			list.Gap(48);
			list.Label("RRD.AnxietyChance".Translate()+"  "+(int)anxietyChance+"%");
			anxietyChance = list.Slider(anxietyChance, 0f, 500.99f);
			list.Gap();
			list.Label("RRD.COCDChance".Translate()+"  "+(int)cocdChance+"%");
			cocdChance = list.Slider(cocdChance, 0f, 500.99f);
			list.Gap();
			list.Label("RRD.DepressionChance".Translate()+"  "+(int)depressionChance+"%");
			depressionChance = list.Slider(depressionChance, 0f, 500.99f);
			list.Gap();
			list.Label("RRD.PTSDChance".Translate()+"  "+(int)ptsdChance+"%");
			ptsdChance = list.Slider(ptsdChance, 0f, 500.99f);
			list.Gap(48);
			list.Label("RRD.CounselingEffectiveness".Translate()+"  "+(int)counselingEffectiveness+"%");
			counselingEffectiveness = list.Slider(counselingEffectiveness, 10f, 300.99f);
			list.Gap();
			list.End();
		}
		public override void ExposeData() {
			base.ExposeData();
			Scribe_Values.Look(ref anxietyChance, "anxietyChance", 100.0f);
			Scribe_Values.Look(ref cocdChance, "cocdChance", 100.0f);
			Scribe_Values.Look(ref depressionChance, "depressionChance", 100.0f);
			Scribe_Values.Look(ref ptsdChance, "ptsdChance", 100.0f);
			Scribe_Values.Look(ref counselingEffectiveness, "counselingEffectiveness", 100.0f);
		}
	}	

	[DefOf]
	public static class DiseaseDefOfRimDisorders {
		public static HediffDef Autism;
		public static HediffDef COCD;
		public static HediffDef Counseled;
		public static HediffDef GeneralizedAnxiety;
		public static HediffDef MajorDepression;
		public static HediffDef PTSD;
		public static HediffDef Refractory;
		public static MentalStateDef Binging_Food;
		public static MentalStateDef COCDCleaning;
		public static MentalStateDef PanicAttack;
		public static MentalStateDef SuicideAttempt;
		public static RecipeDef CounselCOCD;
		public static RecipeDef CounselDepression;
		public static RecipeDef CounselGAD;
		public static RecipeDef CounselPTSD;
		public static ThoughtDef AutismForcedWork;
		public static ThoughtDef AutismForcedWorkSevere;
	}

	public class Watcher : MapComponent {
		private int currentTick;
		public Watcher(Map map) : base(map) { }
		public override void MapComponentTick() {
			base.MapComponentTick();
			currentTick = Find.TickManager.TicksGame;
			if (currentTick % 100 == 99) {
				MentalIllnessGiver.CheckAllPawnsForTriggers();
			}
		}
	}

	public class Alert_SudicideRisk : Alert_Critical {
		public Alert_SudicideRisk() { }
		public override TaggedString GetExplanation() {
			return new TaggedString(Translator.Translate("RRD.SuicideRiskDesc"));
		}
		public override string GetLabel() {
			return Translator.Translate("RRD.SuicideRisk");
		}
		public override AlertReport GetReport() {
			AlertReport alertReport;
			Pawn suidiceRisk = this.GetSuidiceRisk();
			if (suidiceRisk != null) {
				alertReport = AlertReport.CulpritIs(suidiceRisk);
			}
			else {
				alertReport = false;
			}
			return alertReport;
		}
		private Pawn GetSuidiceRisk() {
			Pawn pawn;
			foreach (Map map in Find.Maps) {
				foreach (Pawn freeColonistsAndPrisonersSpawned in map.mapPawns.FreeColonistsAndPrisonersSpawned) {
					if (freeColonistsAndPrisonersSpawned.mindState.mentalBreaker.BreakMajorIsImminent) {
						foreach (Hediff hediff in freeColonistsAndPrisonersSpawned.health.hediffSet.hediffs) {
							if ((object)hediff.def == (object)DiseaseDefOfRimDisorders.MajorDepression && hediff.CurStageIndex == 3) {
								pawn = freeColonistsAndPrisonersSpawned;
								return pawn;
							}
						}
					}
				}
			}
			pawn = null;
			return pawn;
		}
	}

	public class Hediff_ADHD : MentalIllness {
		public Hediff_ADHD() { }
		public override void DoSeverityAction(int index) {
			if (index > 0) {
				if (RestUtility.Awake(this.pawn)) {
					if (Rand.Value < (float)index * 5E-05f) {
						if (this.pawn.Drafted) {
							this.pawn.drafter.Drafted = false;
						}
						else if (this.pawn != null && this.pawn.needs != null && this.pawn.needs.joy != null && this.pawn.needs.joy.tolerances != null && this.pawn.jobs != null && this.pawn.jobs.jobQueue != null) {
							Log.Message("ADHD tick event");
							Job joyJob = this.GetJoyJob();
							if (joyJob == null) {
								return;
							}
							joyJob.playerForced = true;
							joyJob.ignoreJoyTimeAssignment = true;
							this.pawn.jobs.jobQueue.EnqueueFirst(joyJob, new JobTag?(JobTag.Idle));
							this.pawn.jobs.curDriver.EndJobWith(JobCondition.InterruptOptional);
						}
					}
				}
			}
		}
		private Job GetJoyJob() {
			Job job;
			List<JoyGiverDef> allDefsListForReading = DefDatabase<JoyGiverDef>.AllDefsListForReading;
			IEnumerable<JoyGiverDef> joyGiverDefs = 
				from j in allDefsListForReading
				where j.Worker.MissingRequiredCapacity(this.pawn) == null
				select j;
			if (joyGiverDefs.Count<JoyGiverDef>() != 0) {
				JoyGiverDef joyGiverDef = GenCollection.RandomElement<JoyGiverDef>(joyGiverDefs);
				job = joyGiverDef.Worker.TryGiveJob(this.pawn);
			}
			else {
				Log.Message("No joygiverdefs found for ADHD!");
				job = null;
			}
			return job;
		}
	}

	public class Hediff_Autism : MentalIllness {
		public Hediff_Autism() { }
		public override void DoSeverityAction(int index) {
			if (index != 0 && !this.pawn.InMentalState && RestUtility.Awake(this.pawn)) {
				if (index == 1) {
					if (this.pawn != null && this.pawn.jobs != null && this.pawn.jobs.curJob != null && this.pawn.jobs.curJob.playerForced) {
						this.pawn.needs.mood.thoughts.memories.TryGainMemory(DiseaseDefOfRimDisorders.AutismForcedWork, null);
					}
					if (this.pawn.mindState != null && this.pawn.mindState.duty != null && (object)this.pawn.mindState.duty.def == (object)DutyDefOf.Party) {
						if (Rand.Value < 0.0002f) {
							this.pawn.mindState.duty = null;
							LordUtility.GetLord(this.pawn).ownedPawns.Remove(this.pawn);
							this.pawn.jobs.EndCurrentJob(JobCondition.Incompletable, true);
						}
					}
				}
				if (index == 2) {
					if (this.pawn != null && this.pawn.jobs != null && this.pawn.jobs.curJob != null && this.pawn.jobs.curJob.playerForced) {
						this.pawn.needs.mood.thoughts.memories.TryGainMemory(DiseaseDefOfRimDisorders.AutismForcedWorkSevere, null);
					}
					if (this.pawn.mindState != null && this.pawn.mindState.duty != null && (object)this.pawn.mindState.duty.def == (object)DutyDefOf.Party) {
						this.pawn.mindState.duty = null;
						LordUtility.GetLord(this.pawn).ownedPawns.Remove(this.pawn);
						this.pawn.jobs.EndCurrentJob(JobCondition.Incompletable, true);
					}
				}
				if (index == 3) {
					if (this.pawn != null && this.pawn.jobs != null && this.pawn.jobs.curJob != null && this.pawn.jobs.curJob.playerForced) {
						this.pawn.needs.mood.thoughts.memories.TryGainMemory(DiseaseDefOfRimDisorders.AutismForcedWorkSevere, null);
						this.pawn.jobs.EndCurrentJob(JobCondition.QueuedNoLongerValid, true);
					}
					if (this.pawn.mindState != null && this.pawn.mindState.duty != null && (object)this.pawn.mindState.duty.def == (object)DutyDefOf.Party) {
						this.pawn.mindState.duty = null;
						LordUtility.GetLord(this.pawn).ownedPawns.Remove(this.pawn);
						this.pawn.jobs.EndCurrentJob(JobCondition.Incompletable, true);
					}
				}
			}
		}
	}

	public class Hediff_COCD : MentalIllness {
		public Hediff_COCD() { }
		public override void DoSeverityAction(int index) {
			if (!this.pawn.IsPrisoner) {
				if (!this.pawn.mindState.mentalStateHandler.InMentalState) {
					if (index >= 2) {
						if (Rand.Value < 7.5E-05f) {
							if (this.pawn.health.hediffSet.HasHediff(DiseaseDefOfRimDisorders.Refractory)) {
								return;
							}
							this.pawn.mindState.mentalStateHandler.TryStartMentalState(DiseaseDefOfRimDisorders.COCDCleaning, null, false, false, null);
						}
					}
				}
			}
		}
	}

	public class Hediff_Depression : MentalIllness {
		public Hediff_Depression() { }
		public override void DoSeverityAction(int index) {
			DamageInfo? nullable;
			if (!this.pawn.mindState.mentalStateHandler.InMentalState) {
				if (RestUtility.Awake(this.pawn)) {
					if (!this.pawn.health.hediffSet.HasHediff(DiseaseDefOfRimDisorders.Refractory)) {
						if (index >= 2) {
							if (Rand.Value < 5E-05f * (float)index) {
								this.pawn.mindState.mentalStateHandler.TryStartMentalState(DiseaseDefOfRimDisorders.Binging_Food, null, false, false, null);
								nullable = null;
								this.pawn.health.AddHediff(DiseaseDefOfRimDisorders.Refractory, null, nullable);
							}
						}
						if (index >= 3) {
							if (this.pawn.mindState.mentalBreaker.BreakMajorIsImminent || this.pawn.mindState.mentalBreaker.BreakExtremeIsImminent) {
								if (Rand.Value < 0.0001f) {
									if (this.pawn.IsPrisoner) {
										return;
									}
									this.pawn.mindState.mentalStateHandler.TryStartMentalState(DiseaseDefOfRimDisorders.SuicideAttempt, null, false, false, null);
									nullable = null;
									this.pawn.health.AddHediff(DiseaseDefOfRimDisorders.Refractory, null, nullable);
								}
							}
						}
					}
				}
			}
		}
	}

	public class Hediff_GeneralizedAnxiety : MentalIllness {
		public Hediff_GeneralizedAnxiety() { }
		public override void DoSeverityAction(int index) {
			if (!this.pawn.mindState.mentalStateHandler.InMentalState) {
				if (!this.pawn.IsPrisoner) {
					if (RestUtility.Awake(this.pawn)) {
						if (index >= 3) {
							if (Rand.Value < 3.5E-05f) {
								if (this.pawn.health.hediffSet.HasHediff(DiseaseDefOfRimDisorders.Refractory)) {
									return;
								}
								this.pawn.mindState.mentalStateHandler.TryStartMentalState(DiseaseDefOfRimDisorders.PanicAttack, null, false, false, null);
							}
						}
					}
				}
			}
		}
	}

	public class Hediff_PTSD : MentalIllness {
		public Hediff_PTSD() { }
		public override void DoSeverityAction(int index) {
			if (!this.pawn.mindState.mentalStateHandler.InMentalState) {
				if (!this.pawn.IsPrisoner) {
					if (index >= 2) {
						if (Rand.Value < 0.001f * (float)index && GenAI.EnemyIsNear(this.pawn, 24f)) {
							if (this.pawn.MentalState == null || (object)this.pawn.MentalState.def != (object)DiseaseDefOfRimDisorders.PanicAttack) {
								if (this.pawn.health.hediffSet.HasHediff(DiseaseDefOfRimDisorders.Refractory)) {
									return;
								}
								this.pawn.mindState.mentalStateHandler.TryStartMentalState(DiseaseDefOfRimDisorders.PanicAttack, Translator.Translate("RRD.StressFlashback"), true, false, null);
							}
						}
					}
					if (index >= 3) {
						if (Rand.Value < 1E-05f) {
							if (this.pawn.MentalState == null || (object)this.pawn.MentalState.def != (object)DiseaseDefOfRimDisorders.PanicAttack) {
								if (this.pawn.health.hediffSet.HasHediff(DiseaseDefOfRimDisorders.Refractory)) {
									return;
								}
								this.pawn.mindState.mentalStateHandler.TryStartMentalState(DiseaseDefOfRimDisorders.PanicAttack, Translator.Translate("RRD.RandomFlashback"), true, false, null);
							}
						}
					}
				}
			}
		}
	}

	public class HediffComp_MentalIllness : HediffComp {
		private int episodeTicks = 0;
		private float episodeChancePerDay {
			get { return this.Props.episodeFrequency; }
		}
		public HediffCompProperties_MentalIllness Props {
			get { return (HediffCompProperties_MentalIllness)this.props; }
		}
		public HediffComp_MentalIllness() { }
		public override void CompPostTick(ref float severityadjustment) {
			if (this.episodeChancePerDay != 0f) {
				base.CompPostTick(ref severityadjustment);
				if (Rand.Value < this.episodeChancePerDay / 60000f) {
					this.TryTriggerEpisode();
				}
				if (this.parent.CurStageIndex > 0) {
					this.episodeTicks++;
					if ((float)this.episodeTicks > this.Props.maxEpisodeDuration * 60000f) {
						this.EndEpisode();
					}
					else if ((float)this.episodeTicks > this.Props.minEpisodeDuration * 60000f) {
						if (Rand.Value < 0.00015f) {
							this.EndEpisode();
						}
					}
				}
			}
		}
		public override void CompTended(float quality, int batchPosition = 0) {
			if (this.parent.def.tendable) {
				base.CompTended(quality, batchPosition);
				if (quality > Rand.Value) {
					this.parent.Severity = Math.Max(this.parent.Severity - 0.2f, 0f);
				}
			}
		}
		public void EndEpisode() {
			this.parent.Severity = 0.05f;
			this.episodeTicks = 0;
		}
		protected virtual void TryTriggerEpisode() {
			if (this.episodeChancePerDay != 0f) {
				if (this.parent.CurStageIndex == 0) {
					Log.Message(string.Concat("Triggering episode of ", base.Def.defName, " for", base.Pawn.Name.ToStringShort));
					this.parent.Severity = this.Props.minEpisodeStrength + Rand.Value * (this.Props.maxEpisodeStrength - this.Props.minEpisodeStrength);
				}
				else if (this.parent.Severity < this.Props.maxEpisodeStrength) {
					HediffWithComps hediffWithComp = this.parent;
					hediffWithComp.Severity = hediffWithComp.Severity + Rand.Value * this.Props.maxEpisodeStrength;
					this.parent.Severity = Math.Min(this.parent.Severity, this.Props.maxEpisodeStrength);
					HediffCompProperties_MentalIllness props = this.Props;
					props.maxEpisodeStrength = props.maxEpisodeStrength + Rand.Value * 0.2f;
				}
			}
		}
	}

	public class HediffCompProperties_MentalIllness : HediffCompProperties {
		public float episodeFrequency = 0f;
		public float minEpisodeDuration = 2f;
		public float maxEpisodeDuration = 10f;
		public float maxEpisodeStrength = 0f;
		public float minEpisodeStrength = 0f;
		public bool counselable = true;
		public float GetMinStrength {
			get { return Rand.Value * this.maxEpisodeStrength; }
		}
		public float GetNewMaxStrength {
			get { return this.MaxEpisodeStrengthCurve().Evaluate(Rand.Value); }
		}
		public HediffCompProperties_MentalIllness() {
			this.compClass = typeof(HediffComp_MentalIllness);
			this.maxEpisodeStrength = this.GetNewMaxStrength;
			this.minEpisodeStrength = this.GetMinStrength;
		}
		private SimpleCurve MaxEpisodeStrengthCurve() {
			SimpleCurve simpleCurve = new SimpleCurve();
			simpleCurve.Add(new CurvePoint(0f, 0.2f), true);
			simpleCurve.Add(new CurvePoint(0.8f, 0.5f), true);
			simpleCurve.Add(new CurvePoint(1f, 1f), true);
			return simpleCurve;
		}
	}

	public class IncidentWorker_GiveRandomMentalIllness : IncidentWorker {
		private HediffDef RandomMentalIllness {
			get { return DiseaseDefOfRimDisorders.MajorDepression; }
		}
		public IncidentWorker_GiveRandomMentalIllness() { }
		protected override bool CanFireNowSub(IncidentParms parms) {
			return true;
		}
		protected override bool TryExecuteWorker(IncidentParms parms) {
			bool flag;
			Map map = (Map)parms.target;
			List<Pawn> list = map.mapPawns.FreeColonistsAndPrisoners.ToList<Pawn>();
			if (list.Count != 0) {
				GenList.Shuffle<Pawn>(list);
				List<HediffDef> hediffDefs = new List<HediffDef>() {
					DiseaseDefOfRimDisorders.MajorDepression,
					DiseaseDefOfRimDisorders.GeneralizedAnxiety,
					DiseaseDefOfRimDisorders.COCD,
					DiseaseDefOfRimDisorders.PTSD
				};
				if (hediffDefs.Count == 0) {
					flag = false;
				}
				else if (Rand.Value >= 1f) {
					flag = false;
				}
				else {
					DamageInfo? nullable = null;
					list[0].health.AddHediff(GenCollection.RandomElement<HediffDef>(hediffDefs), null, nullable);
					flag = true;
				}
			}
			else {
				flag = false;
			}
			return flag;
		}
	}

	public abstract class MentalIllness : HediffWithComps {
		public static float mult_counsel;
		static MentalIllness() {
			MentalIllness.mult_counsel = 2f * (Controller.Settings.counselingEffectiveness / 100);
		}
		protected MentalIllness() { }
		public virtual void AnyPostTickAction() {
			this.TryAddCounseling();
			this.DoSeverityAction(this.CurStageIndex);
		}
		public void Counsel(int social) {
			HediffComp_MentalIllness hediffCompMentalIllness = HediffUtility.TryGetComp<HediffComp_MentalIllness>(this);
			if (hediffCompMentalIllness.Props.counselable) {
				float value = Rand.Value * (2f + 0.8f * (float)social) / 100f;
				value *= MentalIllness.mult_counsel;
				MentalIllness mentalIllness = this;
				((Hediff)mentalIllness).Severity = ((Hediff)mentalIllness).Severity - value;
				hediffCompMentalIllness.Props.maxEpisodeStrength -= value;
				IntVec3 position = this.pawn.Position;
				MoteMaker.ThrowText(position.ToVector3(), this.pawn.Map, string.Format("{0}: {1:###}%", Translator.Translate("RRD.Improvement"), value * 100f), 3f);
			}
		}
		public abstract void DoSeverityAction(int index);
		public override void PostTick() {
			base.PostTick();
			if (this.pawn != null) {
				this.AnyPostTickAction();
			}
			else {
				Log.Message("Cannot find pawn for hediff");
			}
		}
		public virtual void TryAddCounseling() {
			HediffComp_MentalIllness hediffCompMentalIllness = HediffUtility.TryGetComp<HediffComp_MentalIllness>(this);
			if (this.pawn.Spawned) {
				if (this.pawn.Map != null) {
					if (hediffCompMentalIllness != null) {
						if (this.CurStageIndex != 0) {
							if (hediffCompMentalIllness.Props.counselable) {
								if (!this.pawn.health.hediffSet.HasHediff(HediffDef.Named("Counseled"))) {
									RecipeDef counselDepression = null;
									if ((object)this.def == (object)DiseaseDefOfRimDisorders.MajorDepression) {
										counselDepression = DiseaseDefOfRimDisorders.CounselDepression;
									}
									if ((object)this.def == (object)DiseaseDefOfRimDisorders.GeneralizedAnxiety) {
										counselDepression = DiseaseDefOfRimDisorders.CounselGAD;
									}
									if ((object)this.def == (object)DiseaseDefOfRimDisorders.PTSD) {
										counselDepression = DiseaseDefOfRimDisorders.CounselPTSD;
									}
									if ((object)this.def == (object)DiseaseDefOfRimDisorders.COCD) {
										counselDepression = DiseaseDefOfRimDisorders.CounselCOCD;
									}
									if (counselDepression != null) {
										bool flag = false;
										foreach (Pawn freeColonistsSpawned in this.pawn.Map.mapPawns.FreeColonistsSpawned) {
											if ((object)freeColonistsSpawned != (object)this.pawn) {
												if (!freeColonistsSpawned.Downed) {
													if (!freeColonistsSpawned.Dead) {
														if (counselDepression.PawnSatisfiesSkillRequirements(freeColonistsSpawned)) {
															flag = true;
															break;
														}
													}
												}
											}
										}
										if (!flag && this.pawn.BillStack.Bills.Exists((Bill b) => b.recipe.defName == counselDepression.defName)) {
											List<Bill> bills = new List<Bill>();
											foreach (Bill bill in this.pawn.BillStack.Bills) {
												if (bill.recipe.defName == counselDepression.defName) {
													bills.Add(bill);
												}
											}
											foreach (Bill bill1 in bills) {
												this.pawn.BillStack.Bills.Remove(bill1);
											}
										}
										else if (flag) {
											if (!this.pawn.BillStack.Bills.Exists((Bill b) => b.recipe.defName == counselDepression.defName)) {
												Bill_Medical billMedical = new Bill_Medical(counselDepression);
												this.pawn.BillStack.AddBill(billMedical);
												billMedical.Part = this.pawn.health.hediffSet.GetBrain();
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}
	}

	public static class MentalIllnessGiver {
		public static float rate_dep;
		public static float rate_anx;
		public static float rate_ocd;
		public static float rate_pts;
		static MentalIllnessGiver() {
			MentalIllnessGiver.rate_dep = 0.25f * (Controller.Settings.depressionChance / 100);
			MentalIllnessGiver.rate_anx = 0.25f * (Controller.Settings.anxietyChance / 100);
			MentalIllnessGiver.rate_ocd = 0.25f * (Controller.Settings.cocdChance / 100);
			MentalIllnessGiver.rate_pts = 0.25f * (Controller.Settings.ptsdChance / 100);
		}
		public static void CheckAllPawnsForTriggers() {
			foreach (Map map in Find.Maps) {
				foreach (Pawn list in map.mapPawns.FreeColonistsAndPrisonersSpawned.ToList<Pawn>()) {
					if (!list.Dead) {
						MentalIllnessGiver.CheckPawnForTriggers(list);
					}
				}
			}
		}
		public static void CheckPawnForTriggers(Pawn p) {
			DamageInfo? nullable;
			if (!p.Dead) {
				bool flag = p.needs.mood.thoughts.TotalMoodOffset() < p.mindState.mentalBreaker.BreakThresholdMinor;
				bool flag1 = p.needs.mood.thoughts.TotalMoodOffset() < p.mindState.mentalBreaker.BreakThresholdMajor;
				bool downed = p.Downed;
				bool flag2 = false;
				bool flag3 = false;
				foreach (Hediff hediff in p.health.hediffSet.hediffs) {
					if (hediff is Hediff_Addiction) {
						Hediff_Addiction hediffAddiction = (Hediff_Addiction)hediff;
						if (AddictionUtility.IsAddicted(p, hediffAddiction.Chemical)) {
							if (hediffAddiction.Chemical.defName == "Alcohol" || hediffAddiction.Chemical.defName == "Smokeleaf") {
								flag3 = true;
							}
							if (hediffAddiction.Chemical.defName == "WakeUp" || hediffAddiction.Chemical.defName == "GoJuice" || hediffAddiction.Chemical.defName == "Psychite") {
								flag2 = true;
							}
						}
					}
				}
				//bool flag4 = false;
				bool flag5 = false;
				bool flag6 = false;
				if (p.health != null) {
					if (p.health.InPainShock) {
						//flag4 = true;
					}
					if (p.health.hediffSet != null && p.health.hediffSet.AnyHediffMakesSickThought) {
						flag5 = true;
					}
				}
				bool flag7 = false;
				bool flag8 = false;
				bool flag9 = false;
				bool flag10 = false;
				foreach (Thought_Memory memory in p.needs.mood.thoughts.memories.Memories) {
					if ((object)memory.def == (object)ThoughtDefOf.WitnessedDeathAlly || (object)memory.def == (object)ThoughtDefOf.WitnessedDeathFamily || (object)memory.def == (object)ThoughtDefOf.WitnessedDeathNonAlly && p.story.traits.HasTrait(TraitDefOf.Kind)) {
						flag7 = true;
						flag10 = true;
					}
					if ((object)memory.def == (object)ThoughtDefOf.ObservedLayingCorpse) {
						flag9 = true;
						flag7 = true;
					}
					if ((object)memory.def == (object)ThoughtDefOf.ObservedLayingRottingCorpse) {
						flag9 = true;
						flag8 = true;
						flag7 = true;
					}
					if (memory.MoodOffset() <= -20f) {
						flag6 = true;
					}
				}
				if (flag || Rand.Value < 0.1f) {
					float rateAnx = 3.5E-05f * MentalIllnessGiver.rate_anx;
					if (flag1) {
						rateAnx *= 2f;
					}
					if (flag2) {
						rateAnx *= 3f;
					}
					if (flag6) {
						rateAnx *= 3f;
					}
					if (p.story.traits.HasTrait(TraitDefOf.Nerves)) {
						if (p.story.traits.GetTrait(TraitDefOf.Nerves).Degree == 1) {
							rateAnx /= 5f;
						}
						if (p.story.traits.GetTrait(TraitDefOf.Nerves).Degree == 2) {
							rateAnx /= 16f;
						}
					}
					if (Rand.Value < rateAnx && !p.health.hediffSet.HasHediff(DiseaseDefOfRimDisorders.GeneralizedAnxiety)) {
						nullable = null;
						p.health.AddHediff(DiseaseDefOfRimDisorders.GeneralizedAnxiety, null, nullable);
						string str = string.Format("{0} {1}", p.Name.ToStringShort, Translator.Translate("RRD.DevelopedGAD"));
						if (!flag2) {
							str = (!flag1 ? string.Concat(str, ".") : string.Concat(str, (p.gender != Gender.Male ? Translator.Translate("RRD.StressF") : Translator.Translate("RRD.StressM"))));
						}
						else {
							str = string.Concat(str, Translator.Translate("RRD.StimAbuse"));
						}
						Find.LetterStack.ReceiveLetter(Translator.Translate("RRD.Anxiety"), str, LetterDefOf.NegativeEvent, null);
					}
				}
				if (flag || flag7 || Rand.Value < 0.1f) {
					float rateDep = 3.5E-05f * MentalIllnessGiver.rate_dep;
					if (flag1) {
						rateDep *= 2f;
					}
					if (flag6) {
						rateDep *= 2f;
					}
					if (flag3) {
						rateDep *= 2f;
					}
					if (p.story.traits.HasTrait(TraitDefOf.Nerves)) {
						if (p.story.traits.GetTrait(TraitDefOf.Nerves).Degree == 1) {
							rateDep /= 5f;
						}
						if (p.story.traits.GetTrait(TraitDefOf.Nerves).Degree == 2) {
							rateDep /= 16f;
						}
					}
					if (Rand.Value < rateDep && !p.health.hediffSet.HasHediff(DiseaseDefOfRimDisorders.MajorDepression)) {
						nullable = null;
						p.health.AddHediff(DiseaseDefOfRimDisorders.MajorDepression, null, nullable);
						string str1 = string.Format("{0} {1}", p.Name.ToStringShort, Translator.Translate("RRD.DevelopedDepression"));
						if (flag7) {
							if (!flag10) {
								str1 = (!flag9 ? string.Concat(str1, ".") : string.Concat(str1, Translator.Translate("RRD.SeenCorpse")));
							}
							else {
								str1 = string.Concat(str1, (p.gender != Gender.Male ? Translator.Translate("RRD.WitnessedDeathF") : Translator.Translate("RRD.WitnessedDeathM")));
							}
						}
						else if (!flag1) {
							str1 = (!flag3 ? string.Concat(str1, ".") : string.Concat(str1, Translator.Translate("RRD.DepressantAddiction")));
						}
						else {
							str1 = string.Concat(str1, (p.gender != Gender.Male ? Translator.Translate("RRD.StressF") : Translator.Translate("RRD.StressM")));
						}
						Find.LetterStack.ReceiveLetter(Translator.Translate("RRD.Depression"), str1, LetterDefOf.NegativeEvent, null);
					}
				}
				if (flag || flag8 || flag5 || Rand.Value < 0.05f) {
					float rateOcd = 2.5E-05f * MentalIllnessGiver.rate_ocd;
					if (flag1) {
						rateOcd *= 1.5f;
					}
					if (flag2) {
						rateOcd *= 1.2f;
					}
					if (flag6) {
						rateOcd *= 1.4f;
					}
					if (flag8) {
						rateOcd *= 2f;
					}
					if (p.story.traits.HasTrait(TraitDefOf.Nerves)) {
						if (p.story.traits.GetTrait(TraitDefOf.Nerves).Degree == 1) {
							rateOcd /= 5f;
						}
						if (p.story.traits.GetTrait(TraitDefOf.Nerves).Degree == 2) {
							rateOcd /= 16f;
						}
					}
					if (Rand.Value < rateOcd && !p.health.hediffSet.HasHediff(DiseaseDefOfRimDisorders.COCD)) {
						nullable = null;
						p.health.AddHediff(DiseaseDefOfRimDisorders.COCD, null, nullable);
						string str2 = string.Format(string.Concat("{0} ", Translator.Translate("RRD.DevelopedCOCD")), p.Name.ToStringShort);
						if (flag8) {
							str2 = string.Concat(str2, Translator.Translate("RRD.GrossMortality"));
						}
						else if (!flag5) {
							str2 = (!flag1 ? string.Concat(str2, ".") : string.Concat(str2, (p.gender != Gender.Male ? Translator.Translate("RRD.StressF") : Translator.Translate("RRD.StressM"))));
						}
						else {
							str2 = string.Concat(str2, Translator.Translate("RRD.AfterSick"));
						}
						Find.LetterStack.ReceiveLetter(Translator.Translate("RRD.COCD"), str2, LetterDefOf.NegativeEvent, null);
					}
				}
				if (flag10 && Rand.Value < 0.05f || downed) {
					float ratePts = 0.0025f * MentalIllnessGiver.rate_pts;
					if (flag6) {
						ratePts *= 2f;
					}
					if (p.story.traits.HasTrait(TraitDefOf.Nerves)) {
						if (p.story.traits.GetTrait(TraitDefOf.Nerves).Degree == 1) {
							ratePts /= 5f;
						}
						if (p.story.traits.GetTrait(TraitDefOf.Nerves).Degree == 2) {
							ratePts /= 16f;
						}
					}
					if (Rand.Value < ratePts && !p.health.hediffSet.HasHediff(DiseaseDefOfRimDisorders.PTSD)) {
						nullable = null;
						p.health.AddHediff(DiseaseDefOfRimDisorders.PTSD, null, nullable);
						string str3 = string.Format(string.Concat("{0}", Translator.Translate("RRD.DevelopedPTSD")), p.Name.ToStringShort);
						if (!flag10) {
							str3 = (!downed ? string.Concat(str3, ".") : string.Concat(str3, Translator.Translate("RRD.AfterDowned")));
						}
						else {
							str3 = string.Concat(str3, Translator.Translate("RRD.WitnessedTraumaticDeath"));
						}
						Find.LetterStack.ReceiveLetter(Translator.Translate("RRD.PTSD"), str3, LetterDefOf.NegativeEvent, null);
					}
				}
			}
		}
	}

	public class MentalState_COCDCleaning : MentalState {
		public MentalState_COCDCleaning() { }
		public override void MentalStateTick() {
			JobTag? nullable;
			if ((object)this.pawn.jobs.curJob.def != (object)JobDefOf.Clean) {
				List<Thing> filthInHomeArea = this.pawn.Map.listerFilthInHomeArea.FilthInHomeArea;
				if (filthInHomeArea.Count<Thing>() != 0) {
					Thing thing = GenCollection.RandomElement<Thing>(filthInHomeArea);
					nullable = null;
					this.pawn.jobs.StartJob(new Job(JobDefOf.Clean, thing), JobCondition.InterruptForced, null, false, true, null, nullable);
				}
				else {
					nullable = null;
					this.pawn.jobs.StartJob(new Job(JobDefOf.Wait, this.pawn.Position), JobCondition.InterruptForced, null, false, true, null, nullable);
				}
			}
			base.MentalStateTick();
		}
		public override void PostEnd() {
			base.PostEnd();
			this.pawn.health.AddHediff(DiseaseDefOfRimDisorders.Refractory, null, null);
		}
		public override RandomSocialMode SocialModeMax() {
			return 0;
		}
	}

	public class MentalState_PanicAttack : MentalState {
		public MentalState_PanicAttack() { }
		public override void MentalStateTick() {
			JobTag? nullable;
			if (this.pawn.needs.rest.CurCategory != RestCategory.Exhausted) {
				if (Rand.Value < 0.008f && (object)this.pawn.jobs.curJob.def == (object)JobDefOf.FleeAndCower) {
					nullable = null;
					this.pawn.jobs.StartJob(new Job(JobDefOf.FleeAndCower, GenCollection.RandomElement<Thing>(this.pawn.Map.listerThings.AllThings)), JobCondition.InterruptForced, null, false, true, null, nullable);
				}
				else if (Rand.Value < 0.012f && (object)this.pawn.jobs.curJob.def == (object)JobDefOf.FleeAndCower) {
					nullable = null;
					this.pawn.jobs.StartJob(new Job(JobDefOf.FleeAndCower, this.pawn.Position), JobCondition.InterruptForced, null, false, true, null, nullable);
				}
				if ((object)this.pawn.jobs.curJob.def != (object)JobDefOf.FleeAndCower) {
					nullable = null;
					this.pawn.jobs.StartJob(new Job(JobDefOf.FleeAndCower, GenCollection.RandomElement<Thing>(this.pawn.Map.listerThings.AllThings)), JobCondition.InterruptForced, null, false, true, null, nullable);
				}
				base.MentalStateTick();
			}
		}
		public override void PostEnd() {
			base.PostEnd();
			this.pawn.health.AddHediff(DiseaseDefOfRimDisorders.Refractory, null, null);
		}
		public override RandomSocialMode SocialModeMax() {
			return 0;
		}
	}

	public class MentalState_SuicideAttempt : MentalState {
		public MentalState_SuicideAttempt() { }
		public override void MentalStateTick() {
			if ((object)this.pawn.jobs.curJob.def != (object)JobDefOf.Wait_Combat) {
				JobTag? nullable = null;
				this.pawn.jobs.StartJob(new Job(JobDefOf.Wait, this.pawn.Position), JobCondition.InterruptForced, null, false, true, null, nullable);
			}
			if (Rand.Value < 0.005f) {
				BodyPartRecord brain = this.pawn.health.hediffSet.GetBrain();
				foreach (BodyPartRecord allPart in this.pawn.RaceProps.body.AllParts) {
					if ((object)allPart.def == (object)BodyPartDefOf.Neck) {
						brain = allPart;
						break;
					}
				}
				if (Rand.Value >= 0.2f) {
					DamageInfo damageInfo = new DamageInfo(DamageDefOf.Cut, 200, 200, -1f, this.pawn, brain, null, 0);
					this.pawn.TakeDamage(damageInfo);
				}
				else {
					DamageInfo damageInfo1 = new DamageInfo(DamageDefOf.Cut, 1, 1, -1f, this.pawn, brain, null, 0);
					this.pawn.TakeDamage(damageInfo1);
				}
			}
			base.MentalStateTick();
		}
		public override void PostEnd() {
			base.PostEnd();
			this.pawn.health.AddHediff(DiseaseDefOfRimDisorders.Refractory, null, null);
		}
		public override RandomSocialMode SocialModeMax() {
			return 0;
		}
	}

	public class Recipe_Counseling : RecipeWorker {
		public Recipe_Counseling() { }
		public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill) {
			base.ApplyOnPawn(pawn, part, billDoer, ingredients, bill);
			string str = this.recipe.defName;
			HediffDef majorDepression = null;
			if (str == "CounselDepression") {
				majorDepression = DiseaseDefOfRimDisorders.MajorDepression;
			}
			if (str == "CounselGAD") {
				majorDepression = DiseaseDefOfRimDisorders.GeneralizedAnxiety;
			}
			if (str == "CounselPTSD") {
				majorDepression = DiseaseDefOfRimDisorders.PTSD;
			}
			if (str == "CounselCOCD") {
				majorDepression = DiseaseDefOfRimDisorders.COCD;
			}
			if (majorDepression != null) {
				MentalIllness firstHediffOfDef = (MentalIllness)pawn.health.hediffSet.GetFirstHediffOfDef(majorDepression, false);
				firstHediffOfDef.Counsel(billDoer.skills.GetSkill(SkillDefOf.Social).Level);
				if (HediffUtility.TryGetComp<HediffComp_MentalIllness>(firstHediffOfDef).Props.maxEpisodeStrength < firstHediffOfDef.def.stages[1].minSeverity) {
					pawn.health.RemoveHediff(firstHediffOfDef);
					string str1 = string.Format("{1} {0}", Translator.Translate("RRD.CuredText"), pawn.Name.ToStringShort);
					Messages.Message(str1, pawn, MessageTypeDefOf.PositiveEvent);
				}
				DamageInfo? nullable = null;
				pawn.health.AddHediff(HediffDef.Named("Counseled"), null, nullable);
			}
		}
		public override string GetLabelWhenUsedOn(Pawn pawn, BodyPartRecord part) {
			string str;
			if (this.recipe.defName == "CounselDepression") {
				str = Translator.Translate("RRD.CounselDepression");
			}
			else if (this.recipe.defName == "CounselGAD") {
				str = Translator.Translate("RRD.CounselAnxiety");
			}
			else if (this.recipe.defName != "CounselPTSD") {
				str = (this.recipe.defName != "CounselCOCD" ? "" : Translator.Translate("RRD.CounselCOCD").ToString());
			}
			else {
				str = Translator.Translate("RRD.CounselPTSD");
			}
			return str;
		}
		public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn, RecipeDef recipe) {
			IEnumerable<BodyPartRecord> bodyPartRecords;
			List<BodyPartRecord> bodyPartRecords1 = new List<BodyPartRecord>();
			if (!pawn.health.hediffSet.HasHediff(HediffDef.Named("Counseled"))) {
				if (pawn.health.hediffSet.HasHediff(HediffDef.Named(recipe.removesHediff.defName))) {
					bodyPartRecords1.Add(pawn.health.hediffSet.GetBrain());
				}
				bodyPartRecords = bodyPartRecords1;
			}
			else {
				bodyPartRecords = bodyPartRecords1;
			}
			return bodyPartRecords;
		}
	}

	public class ThoughtWorker_AutismCrowded : ThoughtWorker {
		public ThoughtWorker_AutismCrowded() { }
		protected override ThoughtState CurrentStateInternal(Pawn p) {
			ThoughtState inactive;
			if (p.health.hediffSet.HasHediff(DiseaseDefOfRimDisorders.Autism)) {
				MentalIllness firstHediffOfDef = (MentalIllness)p.health.hediffSet.GetFirstHediffOfDef(DiseaseDefOfRimDisorders.Autism, false);
				int curStageIndex = firstHediffOfDef.CurStageIndex;
				if (curStageIndex < 1) {
					inactive = ThoughtState.Inactive;
				}
				else if (RegionAndRoomQuery.GetRoom(p, RegionType.Set_Passable) != null) {
					int num = 0;
					foreach (Pawn allPawnsSpawned in p.Map.mapPawns.AllPawnsSpawned) {
						if (RestUtility.Awake(allPawnsSpawned) && allPawnsSpawned.RaceProps.Humanlike && (object)RegionAndRoomQuery.GetRoom(allPawnsSpawned, RegionType.Set_Passable) == (object)RegionAndRoomQuery.GetRoom(p, RegionType.Set_Passable)) {
							if (allPawnsSpawned.Position.InHorDistOf(p.Position, 10f)) {
								num++;
							}
						}
					}
					if (curStageIndex == 1 && num > 9) {
						inactive = ThoughtState.ActiveAtStage(0);
					}
					else if (curStageIndex != 2 || num <= 5) {
						if (curStageIndex == 3 && num > 4) {
							inactive = ThoughtState.ActiveAtStage(2);
							return inactive;
						}
						inactive = ThoughtState.Inactive;
						return inactive;
					}
					else {
						inactive = ThoughtState.ActiveAtStage(1);
					}
				}
				else {
					inactive = ThoughtState.Inactive;
				}
			}
			else {
				inactive = ThoughtState.Inactive;
				return inactive;
			}
			return inactive;
		}
	}

	public class ThoughtWorker_COCDThought : ThoughtWorker {
		public ThoughtWorker_COCDThought() { }
		protected override ThoughtState CurrentStateInternal(Pawn p) {
			ThoughtState inactive;
			if (!p.health.hediffSet.HasHediff(DiseaseDefOfRimDisorders.COCD)) {
				inactive = ThoughtState.Inactive;
			}
			else {
				MentalIllness firstHediffOfDef = (MentalIllness)p.health.hediffSet.GetFirstHediffOfDef(DiseaseDefOfRimDisorders.COCD, false);
				int curStageIndex = firstHediffOfDef.CurStageIndex;
				if (curStageIndex != 0) {
					inactive = (RegionAndRoomQuery.GetRoom(p, RegionType.Set_Passable).GetStat(RoomStatDefOf.Cleanliness) < 0f ? ThoughtState.ActiveAtStage(curStageIndex - 1) : ThoughtState.Inactive);
				}
				else {
					inactive = ThoughtState.Inactive;
				}
			}
			return inactive;
		}
	}

	public class ThoughtWorker_CounseledThought : ThoughtWorker {
		public ThoughtWorker_CounseledThought() { }
		protected override ThoughtState CurrentStateInternal(Pawn p) {
			ThoughtState thoughtState;
			thoughtState = (!p.health.hediffSet.HasHediff(DiseaseDefOfRimDisorders.Counseled) ? ThoughtState.Inactive : ThoughtState.ActiveAtStage(0));
			return thoughtState;
		}
	}

	public class ThoughtWorker_GeneralizedAnxietyThought : ThoughtWorker {
		public ThoughtWorker_GeneralizedAnxietyThought() { }
		protected override ThoughtState CurrentStateInternal(Pawn p) {
			ThoughtState inactive;
			if (!p.health.hediffSet.HasHediff(DiseaseDefOfRimDisorders.GeneralizedAnxiety)) {
				inactive = ThoughtState.Inactive;
			}
			else {
				MentalIllness firstHediffOfDef = (MentalIllness)p.health.hediffSet.GetFirstHediffOfDef(DiseaseDefOfRimDisorders.GeneralizedAnxiety, false);
				int curStageIndex = firstHediffOfDef.CurStageIndex;
				inactive = (curStageIndex != 0 ? ThoughtState.ActiveAtStage(curStageIndex - 1) : ThoughtState.Inactive);
			}
			return inactive;
		}
	}

	public class ThoughtWorker_MajorDepressionThought : ThoughtWorker {
		public ThoughtWorker_MajorDepressionThought() { }
		protected override ThoughtState CurrentStateInternal(Pawn p) {
			ThoughtState inactive;
			if (!p.health.hediffSet.HasHediff(DiseaseDefOfRimDisorders.MajorDepression)) {
				inactive = ThoughtState.Inactive;
			}
			else {
				MentalIllness firstHediffOfDef = (MentalIllness)p.health.hediffSet.GetFirstHediffOfDef(DiseaseDefOfRimDisorders.MajorDepression, false);
				int curStageIndex = firstHediffOfDef.CurStageIndex;
				inactive = (curStageIndex != 0 ? ThoughtState.ActiveAtStage(curStageIndex - 1) : ThoughtState.Inactive);
			}
			return inactive;
		}
	}

}
