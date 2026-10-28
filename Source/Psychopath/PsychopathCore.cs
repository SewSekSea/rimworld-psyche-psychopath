using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Psychopath
{
    [DefOf]
    public static class PS_DefOf
    {
        public static ThoughtDef PS_Unbothered;
        public static ThoughtDef PS_DidntEvenFlinch;
        public static ThoughtDef PS_GladItWasThem;

        static PS_DefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(PS_DefOf));
        }
    }

    /// <summary>All the tuning numbers in one place. The ones players can change come from the settings menu.</summary>
    public static class Tuning
    {
        private static PsychopathSettings S => PsychopathMod.Settings;

        /// <summary>Multiplier on how much being the steady one lifts them.</summary>
        public static float SteadinessPower => S.steadinessPower;

        /// <summary>Mood at or below which a colonist counts as struggling.</summary>
        public static float StrugglingBelow => S.strugglingBelow;

        /// <summary>Fraction of the colony that has to be struggling before the top stage applies.</summary>
        public static float WorstAtFraction => S.worstAtFraction;

        public static bool Steadiness => S.steadiness;
        public static bool TheyNotice => S.theyNotice;
        public static bool Useful => S.useful;

        /// <summary>How often the colony is looked at.</summary>
        public const int CheckInterval = 5000;

        /// <summary>A colonist has to be this unhappy before noticing who is not.</summary>
        public const float NoticingBelow = 0.45f;

        /// <summary>Stages of PS_Unbothered, low to high.</summary>
        public const int Stages = 5;
    }

    [StaticConstructorOnStartup]
    public static class PS
    {
        /// <summary>Things nobody else wanted to do, which a psychopath does without losing sleep.</summary>
        public static readonly HashSet<ThoughtDef> GrimDeeds = new HashSet<ThoughtDef>();

        static PS()
        {
            Fill(GrimDeeds, "KilledColonist", "ButcheredHumanlikeCorpse", "ObservedLayingCorpse",
                 "HarvestedOrgan", "SoldPrisoner", "ExecutedPrisoner", "GU_Executed", "GU_Harvested");

            new Harmony("psychopath.absence").PatchAll();
            Bridges.Connect();
            Log.Message($"[Psychopath] Loaded. Watching who is fine when nobody else is." + Bridges.Report());
        }

        private static void Fill(HashSet<ThoughtDef> set, params string[] names)
        {
            foreach (string n in names)
            {
                ThoughtDef d = DefDatabase<ThoughtDef>.GetNamedSilentFail(n);
                if (d != null) { set.Add(d); }
            }
        }

        public static bool Counts(Pawn p) =>
            p != null && !p.Dead && p.Spawned && p.needs?.mood != null && p.IsColonist && !p.IsPrisoner;

        /// <summary>
        /// The trait this mod is about, which is the base game's own.
        ///
        /// No new trait is added. Every colony already has psychopaths in it and every player
        /// already knows what the word means there; adding a second one would only produce two
        /// things with the same name and a load-order argument about which wins.
        /// </summary>
        public static bool IsPsychopath(Pawn p) =>
            p?.story?.traits != null && p.story.traits.HasTrait(TraitDefOf.Psychopath);

        /// <summary>Somebody having a bad enough time for it to be visible to the people around them.</summary>
        public static bool IsStruggling(Pawn p) =>
            Counts(p) && p.needs.mood.CurLevelPercentage <= Tuning.StrugglingBelow;
    }
}
