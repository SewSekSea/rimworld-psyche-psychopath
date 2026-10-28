using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace Psychopath
{
    /// <summary>Scaled by the settings slider, so a player can decide how much being the steady one is worth.</summary>
    public class Thought_Unbothered : Thought_Situational
    {
        public override float MoodOffset() => base.MoodOffset() * Tuning.SteadinessPower;
    }

    /// <summary>
    /// How conspicuously fine they are.
    ///
    /// Reads the rest of the colony rather than the psychopath: the stage goes up as more of the
    /// people around them fall apart. A psychopath in a contented colony gets nothing at all from
    /// this. A psychopath in one that is coming apart is having the best year of their life.
    /// </summary>
    public class ThoughtWorker_Unbothered : ThoughtWorker
    {
        protected override ThoughtState CurrentStateInternal(Pawn p)
        {
            if (!Tuning.Steadiness || !PS.Counts(p) || !PS.IsPsychopath(p))
            {
                return ThoughtState.Inactive;
            }
            Map map = p.Map;
            if (map == null)
            {
                return ThoughtState.Inactive;
            }

            int others = 0;
            int struggling = 0;
            foreach (Pawn other in map.mapPawns.FreeColonistsSpawned)
            {
                if (other == p || !PS.Counts(other))
                {
                    continue;
                }
                // Another psychopath is not evidence of anything. They are fine too.
                if (PS.IsPsychopath(other))
                {
                    continue;
                }
                others++;
                if (PS.IsStruggling(other))
                {
                    struggling++;
                }
            }
            // Alone, or with nobody to be unlike, there is nothing to feel steady against.
            if (others == 0)
            {
                return ThoughtState.Inactive;
            }

            float fraction = struggling / (float)others;
            float scaled = Tuning.WorstAtFraction <= 0f ? 1f : fraction / Tuning.WorstAtFraction;
            int stage = Mathf.Clamp(Mathf.FloorToInt(scaled * (Tuning.Stages - 1) + 0.0001f),
                0, Tuning.Stages - 1);
            return stage <= 0 ? ThoughtState.Inactive : ThoughtState.ActiveAtStage(stage);
        }
    }
}
