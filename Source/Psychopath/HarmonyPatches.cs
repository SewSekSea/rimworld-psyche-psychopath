using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Psychopath
{
    /// <summary>
    /// The useful half.
    ///
    /// Somebody has to do the things nobody wants to do, and a colonist who will not lie awake
    /// over it afterwards is worth having. Colonists who would have carried it themselves are
    /// quietly relieved it fell to somebody else, and think better of them for it.
    ///
    /// This is deliberately the same event that gives everyone else a conscience in Guilt. The
    /// psychopath takes the job and takes no weight from it; the people who would have are glad.
    /// </summary>
    [HarmonyPatch(typeof(TaleRecorder), nameof(TaleRecorder.RecordTale),
        new[] { typeof(TaleDef), typeof(object[]) })]
    public static class Patch_RecordTale
    {
        /// <summary>Tales the colony would rather somebody else had been responsible for.</summary>
        private static readonly HashSet<string> Grim = new HashSet<string>
        {
            "ExecutedPrisoner", "ExecutedPrisonerForCrime", "ButcheredHumanlikeCorpse",
            "HarvestedOrgan", "SoldPrisoner", "KilledColonist", "KilledMajorColonyEnemy",
        };

        public static void Postfix(TaleDef def, object[] args)
        {
            if (!Tuning.Useful || def == null || args == null || !Grim.Contains(def.defName))
            {
                return;
            }
            Pawn doer = null;
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] is Pawn p)
                {
                    doer = p;
                    break;
                }
            }
            if (doer == null || !PS.IsPsychopath(doer) || !PS.Counts(doer) || doer.Map == null)
            {
                return;
            }

            foreach (Pawn other in doer.Map.mapPawns.FreeColonistsSpawned)
            {
                if (other == doer || !PS.Counts(other))
                {
                    continue;
                }
                // A colonist who would not have minded either way is not relieved about anything.
                if (PS.IsPsychopath(other) || other.story.traits.HasTrait(TraitDefOf.Bloodlust))
                {
                    continue;
                }
                other.needs.mood.thoughts.memories.TryGainMemory(PS_DefOf.PS_GladItWasThem, doer);
            }
        }
    }
}
