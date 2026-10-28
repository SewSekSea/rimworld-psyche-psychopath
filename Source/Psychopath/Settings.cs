using UnityEngine;
using Verse;

namespace Psychopath
{
    public class PsychopathSettings : ModSettings
    {
        public float steadinessPower = 1f;
        public float strugglingBelow = 0.40f;
        public float worstAtFraction = 0.70f;

        public bool steadiness = true;
        public bool theyNotice = true;
        public bool useful = true;

        public override void ExposeData()
        {
            PsychopathSettings d = new PsychopathSettings();
            Scribe_Values.Look(ref steadinessPower, "steadinessPower", d.steadinessPower);
            Scribe_Values.Look(ref strugglingBelow, "strugglingBelow", d.strugglingBelow);
            Scribe_Values.Look(ref worstAtFraction, "worstAtFraction", d.worstAtFraction);
            Scribe_Values.Look(ref steadiness, "steadiness", true);
            Scribe_Values.Look(ref theyNotice, "theyNotice", true);
            Scribe_Values.Look(ref useful, "useful", true);
        }

        public void Reset()
        {
            PsychopathSettings d = new PsychopathSettings();
            steadinessPower = d.steadinessPower;
            strugglingBelow = d.strugglingBelow;
            worstAtFraction = d.worstAtFraction;
            steadiness = theyNotice = useful = true;
        }
    }

    public class PsychopathMod : Mod
    {
        public static PsychopathSettings Settings = new PsychopathSettings();

        public PsychopathMod(ModContentPack content) : base(content)
        {
            Settings = GetSettings<PsychopathSettings>();
        }

        public override string SettingsCategory() => "Psychopath";

        public override void DoSettingsWindowContents(Rect inRect)
        {
            PsychopathSettings s = Settings;
            Listing_Standard list = new Listing_Standard();
            list.Begin(inRect);

            list.Label("Being the steady one");
            s.steadinessPower = list.SliderLabeled(
                $"Worth {s.steadinessPower:P0} to them", s.steadinessPower, 0.25f, 2f);
            list.Gap(4f);
            s.strugglingBelow = list.SliderLabeled(
                $"A colonist counts as struggling below {s.strugglingBelow:P0} mood",
                s.strugglingBelow, 0.15f, 0.7f);
            list.Gap(4f);
            s.worstAtFraction = list.SliderLabeled(
                $"At its strongest when {s.worstAtFraction:P0} of the colony is struggling",
                s.worstAtFraction, 0.3f, 1f);

            list.Gap(12f);
            list.Label("Optional behaviours");
            list.CheckboxLabeled("Steadiness", ref s.steadiness,
                "The worse everybody else is doing, the better they feel. A psychopath in a contented colony gets nothing from this at all.");
            list.CheckboxLabeled("The colony notices", ref s.theyNotice,
                "Colonists having a hard time register that this person never does, and think less of them for it. Not for anything they did.");
            list.CheckboxLabeled("Glad it was them", ref s.useful,
                "When a psychopath does something nobody else wanted to do, the colonists who would have carried it are relieved it fell to somebody who will not.");

            list.Gap(12f);
            list.Label("This adds no trait. It works on the Psychopath already in the base game, so " +
                       "every colonist who has ever had it in your save is covered.\n\n" +
                       "What a psychopath does not feel is not adjustable, because the base game " +
                       "already decides that. What this adds is what it costs everybody else.");

            list.Gap(12f);
            if (list.ButtonText("Reset to defaults"))
            {
                s.Reset();
            }

            list.End();
        }
    }
}
